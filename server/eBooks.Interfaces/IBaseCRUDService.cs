using eBooks.Models.Search;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IBaseCRUDService<TSearch, TCreate, TUpdate, TResponse> : IBaseReadOnlyService<TSearch, TResponse>
    {
        Task<TResponse> Post(TCreate req);
        Task<TResponse> Put(int id, TUpdate req);
        Task<TResponse> Delete(int id);
    }
}
