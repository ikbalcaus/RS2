using eBooks.Models;

namespace eBooks.Services
{
    public interface IBaseReadOnlyService<TSearch, TResponse> where TSearch : BaseSearch
    {
        public PagedResult<TResponse> GetPaged(TSearch search);
        public TResponse GetById(int id);
    }
}
