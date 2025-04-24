using eBooks.Models;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IBaseService<TSearch, TInsert, TUpdate, TResponse> : IBaseReadOnlyService<TSearch, TResponse> where TSearch : BaseSearch where TResponse : class
    {
        TResponse Insert(TInsert req);
        TResponse Update(int id, TUpdate req);
        void Delete(int id);
    }
}
