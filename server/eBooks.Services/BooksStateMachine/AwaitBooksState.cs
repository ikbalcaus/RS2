using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class AwaitBooksState : BaseBooksState
    {
        public AwaitBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider) : base(db, mapper, serviceProvider)
        {
        }

        public override BooksRes Approve(int id)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            entity.StateMachine = "approve";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public override BooksRes Reject(int id, string message)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            entity.StateMachine = "reject";
            entity.RejectionReason = message;
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }
    }
}
