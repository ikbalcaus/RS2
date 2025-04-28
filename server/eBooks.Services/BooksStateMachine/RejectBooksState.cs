using eBooks.Database;
using eBooks.Database.Models;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class RejectBooksState : BaseBooksState
    {
        public RejectBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider) : base(db, mapper, serviceProvider)
        {
        }

        public override List<string> AllowedActions(Book entity)
        {
            return new List<string>() {};
        }
    }
}
