using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class ApproveBooksState : BaseBooksState
    {
        public ApproveBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider)
            : base(db, mapper, serviceProvider)
        {
        }

        public async override Task<BooksRes> Hide(int id)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            entity.StateMachine = "Hide";
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public async override Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Hide) };
        }
    }
}
