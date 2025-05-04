using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class DraftBooksState : BaseBooksState
    {
        public DraftBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider) : base(db, mapper, serviceProvider)
        {
        }

        public override BooksRes Update(int id, BooksUpdateReq req)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            entity.StateMachine = "draft";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public override BooksRes Await(int id)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            entity.StateMachine = "await";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public override List<string> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Await) };
        }
    }
}
