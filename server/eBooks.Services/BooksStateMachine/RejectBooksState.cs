using eBooks.Database;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class RejectBooksState : BaseBooksState
    {
        public RejectBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider) : base(db, mapper, serviceProvider)
        {
        }
    }
}
