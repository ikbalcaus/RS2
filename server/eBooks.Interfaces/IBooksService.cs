using eBooks.Models.Books;

namespace eBooks.Interfaces
{
    public interface IBooksService : IBaseService<BooksSearch, BooksInsertReq, BooksUpdateReq, BooksRes>
    {
    }
}
