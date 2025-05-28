using System.Security.Claims;
using EasyNetQ;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Messages;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services.BooksStateMachine
{
    public class AwaitBooksState : BaseBooksState
    {
        protected IHttpContextAccessor _httpContextAccessor;
        protected IBus _bus;

        public AwaitBooksState(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBus bus, IServiceProvider serviceProvider, ILogger<BooksService> logger)
            : base(db, mapper, serviceProvider, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _bus = bus;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public override async Task<BooksRes> Update(int id, BooksPutReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            if (req.LanguageId != null && !await _db.Set<Language>().AnyAsync(x => x.LanguageId == req.LanguageId))
                throw new ExceptionNotFound();
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            var filePath = entity.FilePath;
            if (req.BookPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.BookPdfFile, true);
            if (req.PreviewPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.PreviewPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Approve(int id)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.StateMachine = "approve";
            entity.RejectionReason = null;
            entity.ReviewedById = GetUserId();
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            _bus.PubSub.Publish(new BookReviewed { Book = _mapper.Map<BooksRes>(entity), Status = "approved" });
            _bus.PubSub.Publish(new PublisherFollowNotification { Book = _mapper.Map<BooksRes>(entity), Action = "added new" });
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Reject(int id, string message)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.StateMachine = "reject";
            entity.RejectionReason = message;
            entity.ReviewedById = GetUserId();
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} rejected. Reason: {message}");
            _bus.PubSub.Publish(new BookReviewed { Book = _mapper.Map<BooksRes>(entity), Status = "rejected" });
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Approve), nameof(Reject) };
        }
    }
}
