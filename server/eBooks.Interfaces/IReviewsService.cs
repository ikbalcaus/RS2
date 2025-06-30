using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IReviewsService : IBaseUserContextService<ReviewsSearch, ReviewsReq, ReviewsRes>
    {
        Task<ReviewsRes> Put(int bookId, ReviewsReq req);
        Task<ReviewsRes> Report(int userId, int bookId, string reason);
        Task<ReviewsRes> AdminDelete(int userId, int bookId);
    }
}
