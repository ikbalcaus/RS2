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
            var languageId = entity.LanguageId;
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            if (req.PdfFile != null)
                Helpers.UploadPdfFile(entity, req.PdfFile);
            await _db.SaveChangesAsync();
            if (req.Images != null && req.Images.Any())
                Helpers.UploadImages(_db, _mapper, entity.BookId, req.Images);
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
            _bus.PubSub.Publish(new BookReviewed { Book = _mapper.Map<BooksRes>(entity) });
            _bus.PubSub.Publish(new PublisherFollowNotification { Book = _mapper.Map<BooksRes>(entity), Action = "Added new" });
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
            _bus.PubSub.Publish(new BookReviewed { Book = _mapper.Map<BooksRes>(entity) });
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Approve), nameof(Reject) };
        }
    }
}
