using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IBaseUserContextService<TSearch, TRequest, TResponse>
    {
        Task<PagedResult<TResponse>> GetPaged(TSearch search);
        Task<TResponse> GetByBookId(int bookId);
        Task<TResponse> Post(int bookId, TRequest req);
        Task<TResponse> Patch(int bookId);
        Task<TResponse> Delete(int bookId);
    }
}
