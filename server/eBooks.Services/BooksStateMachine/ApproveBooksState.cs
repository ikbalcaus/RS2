using System.Security.Claims;
using EasyNetQ;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Messages;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services.BooksStateMachine
{
    public class ApproveBooksState : BaseBooksState
    {
        protected IBus _bus;

        public ApproveBooksState(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBus bus, IServiceProvider serviceProvider, ILogger<BooksService> logger)
            : base(db, mapper, httpContextAccessor, serviceProvider, logger)
        {
            _bus = bus;
        }

        public override async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.StateMachine = "hide";
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} hidden.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Reject(int id, string message)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.StateMachine = "reject";
            entity.RejectionReason = message;
            entity.ReviewedById = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : 0;
            _logger.LogInformation($"Book with title {entity.Title} rejected. Reason: {message}");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new BookReviewed { Book = result });
            string notificationMessage = $"Your book has been rejected. Reason: {result.RejectionReason}";
            var user = await _db.Set<User>().FirstOrDefaultAsync(x => x.UserId == result.Publisher.UserId);
            var notification = new Notification
            {
                UserId = user.UserId,
                BookId = result.BookId,
                Message = notificationMessage
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
            return result;
        }

        public override List<string> AdminAllowedActions()
        {
            return new List<string>() { nameof(Reject) };
        }

        public override List<string> UserAllowedActions()
        {
            return new List<string>() { nameof(Hide) };
        }
    }
}
