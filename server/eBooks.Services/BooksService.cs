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
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public override async Task<BooksRes> GetById(int id)
        {
            var entity = _db.Set<Book>().FirstOrDefault(x => x.BookId == id);
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
            if (req.PreviewPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.PreviewPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile);
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
            _logger.LogInformation($"Book with title {entity.Title} awaited.");
            return await state.Await(id);
        }

        public async Task<BooksRes> Approve(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            return await state.Approve(id);
        }

        public async Task<BooksRes> Reject(int id, string message)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} rejected with message \"{message}\".");
            return await state.Reject(id, message);
        }

        public async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} is hidden/not hidden.");
            return await state.Hide(id);
        }

        public async Task<List<string>> AllowedActions(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.AllowedActions(entity);
        }

        public async Task<List<string>> BookStates()
        {
            return new List<string> { "Approved", "Awaited", "Drafted", "Hidden", "Rejected" };
        }
    }
}
