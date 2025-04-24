using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class ArchiveBooksState : BaseBooksState
    {
        public ArchiveBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider) : base(db, mapper, serviceProvider)
        {
        }

        public override BooksRes Archive(int id)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            entity.StateMachine = "approve";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }
    }
}
