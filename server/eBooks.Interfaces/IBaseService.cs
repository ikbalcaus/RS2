using eBooks.Models;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IBaseService<TSearch, TCreate, TUpdate, TResponse> : IBaseReadOnlyService<TSearch, TResponse> where TSearch : BaseSearch where TResponse : class
    {
        TResponse Create(TCreate req);
        TResponse Update(int id, TUpdate req);
        TResponse Delete(int id);
    }
}
