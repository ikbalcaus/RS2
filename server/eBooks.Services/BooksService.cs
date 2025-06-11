using System.Security.Claims;
using Azure;
using EasyNetQ;
using EasyNetQ.Internals;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace eBooks.Services
{
    public class BooksService : BaseCRUDService<Book, BooksSearch, BooksPostReq, BooksPutReq, BooksRes>, IBooksService
    {
        protected BaseBooksState _baseBooksState;
        protected IBus _bus;
        protected ILogger<BooksService> _logger;

        public BooksService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, BaseBooksState baseBooksState, IBus bus, ILogger<BooksService> logger)
            : base(db, mapper, httpContextAccessor)
        {
            _baseBooksState = baseBooksState;
            _bus = bus;
            _logger = logger;
        }

        protected async Task<bool> AccessRightExist(int bookId) => await _db.Set<AccessRight>().AnyAsync(x => x.UserId == GetUserId() && x.BookId == bookId);

        public override IQueryable<Book> AddIncludes(IQueryable<Book> query, BooksSearch? search = null)
        {
            query = query.Include(x => x.Publisher);
            query = query.Include(x => x.Language);
            if (search == null || search.IsAuthorsIncluded == true)
                query = query.Include(x => x.BookAuthors).ThenInclude(x => x.Author);
            if (search == null || search.IsGenresIncluded == true)
                query = query.Include(x => x.BookGenres).ThenInclude(x => x.Genre);
            return query;
        }

        public override IQueryable<Book> AddFilters(IQueryable<Book> query, BooksSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.Title))
                query = query.Where(x => x.Title.ToLower().StartsWith(search.Title.ToLower()));
            if (search.PublisherId != null)
                query = query.Where(x => x.PublisherId == search.PublisherId);
            if (!string.IsNullOrWhiteSpace(search.Publisher))
                query = query.Where(x => x.Publisher.UserName.ToLower().StartsWith(search.Publisher.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.Language))
                query = query.Where(x => x.Language.Name.ToLower().StartsWith(search.Language.ToLower()));
            query = search.Status switch
            {
                "Approved" => query.Where(x => x.StateMachine == "approve"),
                "Awaited" => query.Where(x => x.StateMachine == "await"),
                "Drafted" => query.Where(x => x.StateMachine == "draft"),
                "Hidden" => query.Where(x => x.StateMachine == "hide"),
                "Rejected" => query.Where(x => x.StateMachine == "reject"),
                _ => query,
            };
            if (search.IsDeleted == "Not deleted")
                query = query.Where(x => x.DeletionReason == null);
            else if (search.IsDeleted == "Deleted")
                query = query.Where(x => x.DeletionReason != null);
            query = search.OrderBy switch
            {
                "First modified" => query.OrderBy(x => x.ModifiedAt),
                "Title (A-Z)" => query.OrderBy(x => x.Title),
                "Title (Z-A)" => query.OrderByDescending(x => x.Title),
                "Publisher (A-Z)" => query.OrderBy(x => x.Publisher.UserName),
                "Publisher (Z-A)" => query.OrderByDescending(x => x.Publisher.UserName),
                _ => query.OrderByDescending(x => x.ModifiedAt),
            };
            return query;
        }

        public override async Task<PagedResult<BooksRes>> GetPaged(BooksSearch search)
        {
            var query = _db.Set<Book>().AsQueryable();
            query = AddIncludes(query);
            query = AddFilters(query, search);
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true && search.Page.Value > 0)
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            TypeAdapterConfig<Book, BooksRes>.NewConfig().Map(dest => dest.Status, src => MapState(src.StateMachine));
            var result = list.Adapt<List<BooksRes>>();
            var pagedResult = new PagedResult<BooksRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public override async Task<BooksRes> GetById(int id)
        {
            var query = _db.Set<Book>().AsQueryable();
            query = AddIncludes(query);
            var entity = query.FirstOrDefault(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            var result = _mapper.Map<BooksRes>(entity);
            var userId = GetUserId();
            if (userId != entity.PublisherId)
            {
                entity.NumberOfViews += 1;
                await _db.SaveChangesAsync();
            }
            result.HasAccessRight = await AccessRightExist(id);
            result.Status = MapState(entity.StateMachine);
            return result;
        }

        public override async Task<BooksRes> Post(BooksPostReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            if (!await _db.Set<Language>().AnyAsync(x => x.LanguageId == req.LanguageId))
                throw new ExceptionNotFound();
            var entity = _mapper.Map<Book>(req);
            var filePath = $"{Guid.NewGuid():N}";
            entity.PublisherId = GetUserId();
            entity.FilePath = filePath;
            if (req.BookPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.BookPdfFile, true);
            if (req.SummaryPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.SummaryPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile, true);
            _db.Add(entity);
            await _db.SaveChangesAsync();
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
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Update(id, req);
        }

        public override async Task<BooksRes> Delete(int id)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.RejectionReason = "Deleted by user";
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} deleted.");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new BookDeactivated { Book = result });
            return result;
        }

        public async Task<BooksRes> DeleteByAdmin(int id, string? reason)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (entity.DeletionReason == null && reason == null)
                throw new ExceptionBadRequest("Not deleted");
            if (entity.DeletionReason != null && reason != null)
                throw new ExceptionBadRequest("Already deleted");
            entity.DeletionReason = reason;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} deleted.");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new BookDeactivated { Book = result });
            return result;
        }

        public async Task<BooksRes> SetDiscount(int id, DiscountReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (req.DiscountPercentage <= 0 || req.DiscountPercentage >= 100)
                errors.AddError("Discount", "Discount must be between 0 and 100");
            if (req.DiscountStart >= req.DiscountEnd)
                errors.AddError("Discount", "Discount start date must be before discount end date");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            _mapper.Map(req, entity);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} is discounted by {req.DiscountPercentage}%.");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new PublisherFollowing { Book = result, Action = "added discount to a" });
            _bus.PubSub.Publish(new BookDiscounted { Book = result });
            return result;
        }

        public async Task<Tuple<string, byte[]>> GetBookFile(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!await AccessRightExist(id) && entity.PublisherId != GetUserId() && role != "Admin" && role != "Moderator")
                throw new ExceptionForbidden("You do not have access to this book");
            var fileName = $"{entity.FilePath}.pdf";
            var filePath = Path.Combine("wwwroot", "pdfs", "books", fileName);
            if (!System.IO.File.Exists(filePath))
                throw new ExceptionBadRequest($"PDF file does not exist");
            var fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
            return new Tuple<string, byte[]>(fileName, fileContent);
        }

        public async Task<BooksRes> Await(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Await(id);
        }

        public async Task<BooksRes> Approve(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Approve(id);
        }

        public async Task<BooksRes> Reject(int id, string reason)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Reject(id, reason);
        }

        public async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Hide(id);
        }

        public async Task<List<string>> AdminAllowedActions(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.AdminAllowedActions(entity);
        }

        public async Task<List<string>> UserAllowedActions(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.UserAllowedActions(entity);
        }

        public async Task<List<string>> BookStates()
        {
            return ["Approved", "Awaited", "Drafted", "Hidden", "Rejected"];
        }

        protected string MapState(string state)
        {
            return state switch
            {
                "draft" => "Drafted",
                "await" => "Awaited",
                "approve" => "Approved",
                "reject" => "Rejected",
                "hide" => "Hidden",
                _ => state ?? string.Empty
            };
        }
    }
}
