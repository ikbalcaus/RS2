using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IBooksService : IBaseCRUDService<BooksSearch, BooksPostReq, BooksPutReq, BooksRes>
    {
        Task<BooksRes> DeleteByAdmin(int id, string? reason);
        Task<BooksRes> SetDiscount(int id, DiscountReq req);
        Task<Tuple<string, byte[]>> GetBookFile(int id);
        Task<BooksRes> Await(int id);
        Task<BooksRes> Approve(int id);
        Task<BooksRes> Reject(int id, string message);
        Task<BooksRes> Hide(int id);
        Task<List<string>> AllowedActions(int id);
        Task<List<string>> BookStates();
    }
}
