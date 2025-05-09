using eBooks.Models.Books;

namespace eBooks.Interfaces
{
    public interface IBooksService : IBaseService<BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>
    {
        Task<BooksRes> UndoDelete(int id);
        Task<BookImageRes> DeleteImage(int id, int imageId);
        Task<BooksRes> Await(int id);
        Task<BooksRes> Approve(int id);
        Task<BooksRes> Reject(int id, string message);
        Task<BooksRes> Hide(int id);
        Task<List<string>> AllowedActions(int id);
    }
}
