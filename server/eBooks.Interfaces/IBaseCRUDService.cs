using eBooks.Models.SearchObjects;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IBaseCRUDService<TSearch, TCreate, TUpdate, TResponse> : IBaseReadOnlyService<TSearch, TResponse>
        where TSearch : BaseSearch
        where TResponse : class
    {
        Task<TResponse> Create(TCreate req);
        Task<TResponse> Update(int id, TUpdate req);
        Task<TResponse> Delete(int id);
    }
}
