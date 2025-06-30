using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IReportsService : IBaseUserContextService<ReportsSearch, ReportsReq, ReportsRes>
    {
        Task<ReportsRes> AdminDelete(int userId, int bookId);
    }
}
