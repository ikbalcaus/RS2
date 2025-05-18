using eBooks.Models;

namespace eBooks.Interfaces
{
    public interface IBaseUserContextService<TRequest, TResponse>
    {
        Task<PagedResult<TResponse>> GetPaged();
        Task<TResponse> GetByBookId(int bookId);
        Task<TResponse> Post(int bookId, TRequest req);
        Task<TResponse> Patch(int bookId);
        Task<TResponse> Delete(int bookId);
    }
}
