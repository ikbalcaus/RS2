using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services
{
    public class BooksService : BaseService<Book, BooksSearch, BooksInsertReq, BooksUpdateReq, BooksRes>, IBooksService
    {
        public BooksService(EBooksContext db, IMapper mapper) : base(db, mapper)
        {
        }
    }
}
