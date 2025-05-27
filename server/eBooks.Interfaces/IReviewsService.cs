using eBooks.Models.Requests;
using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IReviewService : IBaseUserContextService<ReviewsPostReq, ReviewsRes>
    {
        Task<ReviewsRes> Put(int bookId, ReviewsPutReq req);
        Task<ReviewsRes> DeleteByUserAndBookId(int userId, int bookId);
    }
}
