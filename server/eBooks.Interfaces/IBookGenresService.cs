using eBooks.Models.Requests;
using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IBookGenresService
    {
        Task<List<BookGenresRes>> GetByBookId(int bookId);
        Task<List<BookGenresRes>> Post(int bookId, BookGenresReq req);
        Task<List<BookGenresRes>> Patch(int bookId, BookGenresReq req);
        Task<BookGenresRes> Delete(int bookId, int authorId);
    }
}
