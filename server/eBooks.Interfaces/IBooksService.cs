using eBooks.Models.Books;
using Microsoft.AspNetCore.Http;

namespace eBooks.Interfaces
{
    public interface IBooksService : IBaseService<BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>
    {
        public BookImageRes DeleteImage(int id, int imageId);
        public BooksRes Await(int id);
        public BooksRes Approve(int id);
        public BooksRes Reject(int id, string message);
        public BooksRes Hide(int id);
        public List<string> AllowedActions(int id);
    }
}
