using eBooks.Database;
using eBooks.Models.Messages;
using eBooks.Subscriber.Services;

namespace eBooks.Subscriber.MessageHandlers
{
    public abstract class BaseMessageHandler<T>
    {
        protected EBooksContext _db;
        protected EmailService _emailService;

        public BaseMessageHandler(EBooksContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }
    }
}
