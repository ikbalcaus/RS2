using System.Security.Claims;
using EasyNetQ;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Messages;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using eBooks.Services.BooksStateMachine;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services
{
    public class BooksService : BaseCRUDService<Book, BooksSearch, BooksPostReq, BooksPutReq, BooksRes>, IBooksService
    {
        protected BaseBooksState _baseBooksState;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IBus _bus;
        protected ILogger<BooksService> _logger;

        public BooksService(EBooksContext db, IMapper mapper, BaseBooksState baseBooksState, IHttpContextAccessor httpContextAccessor, IBus bus, ILogger<BooksService> logger)
            : base(db, mapper)
        {
            _baseBooksState = baseBooksState;
            _httpContextAccessor = httpContextAccessor;
            _bus = bus;
            _logger = logger;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public override async Task<IQueryable<Book>> AddIncludes(IQueryable<Book> query)
        {
            return query.Include(x => x.Publisher).Include(x => x.Language).Include(x => x.BookImages);
        }

        public override async Task<PagedResult<BooksRes>> GetPaged(BooksSearch search)
        {
            var result = new List<BooksRes>();
            var query = _db.Set<Book>().AsQueryable();
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true)
                query = query.Skip(search.Page.Value * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            var config = new TypeAdapterConfig();
            config.NewConfig<Book, BooksRes>().Ignore(x => x.PdfPath);
            result = list.Adapt<List<BooksRes>>(config);
            PagedResult<BooksRes> pagedResult = new PagedResult<BooksRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public override async Task<BooksRes> GetById(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var result = _mapper.Map<BooksRes>(entity);
            var userId = GetUserId();
            if (userId != entity.PublisherId)
            {
                entity.NumberOfViews += 1;
                await _db.SaveChangesAsync();
            }
            var accessRight = await _db.Set<AccessRight>().AnyAsync(x => x.UserId == GetUserId() && x.BookId == id);
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!accessRight && entity.PublisherId != userId && role != "Admin" && role != "Moderator")
                result.PdfPath = null;
            return result;
        }

        public override async Task<BooksRes> Post(BooksPostReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            if (!await _db.Set<Language>().AnyAsync(x => x.LanguageId == req.LanguageId))
                throw new ExceptionNotFound();
            var entity = _mapper.Map<Book>(req);
            entity.PublisherId = GetUserId();
            if (req.PdfFile != null)
                Helpers.UploadPdfFile(entity, req.PdfFile);
            _db.Add(entity);
            await _db.SaveChangesAsync();
            if (req.Images != null && req.Images.Any())
                Helpers.UploadImages(_db, _mapper, entity.BookId, req.Images);
            _logger.LogInformation($"Book with title {entity.Title} created.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Put(int id, BooksPutReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            return await state.Update(id, req);
        }

        public override async Task<BooksRes> Delete(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} deleted.");
            return null;
        }

        public async Task<BooksRes> UndoDelete(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.IsDeleted = false;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} undo-deleted.");
            return _mapper.Map<BooksRes>(entity);
        }

        public async Task<BooksRes> SetDiscount(int id, DiscountReq req)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (req.DiscountPercentage <= 0 || req.DiscountPercentage >= 100)
                throw new ExceptionBadRequest("Discount must be between 0 and 100");
            if (req.DiscountStart >= req.DiscountEnd)
                throw new ExceptionBadRequest("Discount start date must be before discount end date");
            _mapper.Map(req, entity);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} is discounted by {req.DiscountPercentage}%.");
            _bus.PubSub.Publish(new PublisherFollowNotification { Book = _mapper.Map<BooksRes>(entity), Action = "added discount to a" });
            _bus.PubSub.Publish(new BookDiscounted { Book = _mapper.Map<BooksRes>(entity) });
            return _mapper.Map<BooksRes>(entity);
        }

        public async Task<BookImageRes> DeleteImage(int id, int imageId)
        {
            if (!await _db.Set<Book>().AnyAsync(x => x.BookId == id))
                throw new ExceptionNotFound();
            var set = _db.Set<BookImage>();
            var bookImage = await set.FirstOrDefaultAsync(img => img.ImageId == imageId && img.BookId == id);
            if (bookImage == null)
                throw new ExceptionNotFound();
            var imagePath = Path.Combine("wwwroot", bookImage.ImagePath.TrimStart('/'));
            if (File.Exists(imagePath)) File.Delete(imagePath);
            set.Remove(bookImage);
            await _db.SaveChangesAsync();
            return null;
        }

        public async Task<BooksRes> Await(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} awaited.");
            return await state.Await(id);
        }

        public async Task<BooksRes> Approve(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            return await state.Approve(id);
        }

        public async Task<BooksRes> Reject(int id, string message)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} rejected with message \"{message}\".");
            return await state.Reject(id, message);
        }

        public async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} is hidden/not hidden.");
            return await state.Hide(id);
        }

        public async Task<List<string>> AllowedActions(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            return await state.AllowedActions(entity);
        }
    }
}
