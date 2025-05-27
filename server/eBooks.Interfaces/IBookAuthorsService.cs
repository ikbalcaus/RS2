using eBooks.Models.Responses;
using eBooks.Models.Requests;

namespace eBooks.Interfaces
{
    public interface IBookAuthorsService
    {
        Task<List<BookAuthorsRes>> GetByBookId(int bookId);
        Task<List<BookAuthorsRes>> Post(int bookId, BookAuthorsReq req);
        Task<List<BookAuthorsRes>> Patch(int bookId, BookAuthorsReq req);
        Task<BookAuthorsRes> Delete(int bookId, int authorId);
    }
}
