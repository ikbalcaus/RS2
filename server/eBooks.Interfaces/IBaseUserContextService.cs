using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IBaseUserContextService<TRequest, TResponse>
    {
        Task<PagedResult<TResponse>> GetPaged(BaseSearch search);
        Task<TResponse> GetByBookId(int bookId);
        Task<TResponse> Post(int bookId, TRequest req);
        Task<TResponse> Patch(int bookId);
        Task<TResponse> Delete(int bookId);
    }
}
