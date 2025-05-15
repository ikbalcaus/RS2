using eBooks.Models;
using eBooks.Models.SearchObjects;

namespace eBooks.Services
{
    public interface IBaseReadOnlyService<TSearch, TResponse>
        where TSearch : BaseSearch
    {
        Task<PagedResult<TResponse>> GetPaged(TSearch search);
        Task<TResponse> GetById(int id);
    }
}
