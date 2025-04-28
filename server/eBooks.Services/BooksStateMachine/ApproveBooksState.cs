using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class ApproveBooksState : BaseBooksState
    {
        public ApproveBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider) : base(db, mapper, serviceProvider)
        {
        }

        public override BooksRes Archive(int id)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            entity.StateMachine = "archive";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public override List<string> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Archive) };
        }
    }
}
