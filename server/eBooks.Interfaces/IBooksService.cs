using eBooks.Database.Models;
using eBooks.Models.Books;

namespace eBooks.Interfaces
{
    public interface IBooksService : IBaseService<BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>
    {
        public BooksRes Await(int id);
        public BooksRes Approve(int id);
        public BooksRes Reject(int id, string message);
        public BooksRes Archive(int id);
        public List<string> AllowedActions(int id);
    }
}
