using eBooks.Models.Responses;

namespace eBooks.Services
{
    public interface IBaseReadOnlyService<TSearch, TResponse>
    {
        Task<PagedResult<TResponse>> GetPaged(TSearch search);
        Task<TResponse> GetById(int id);
    }
}
