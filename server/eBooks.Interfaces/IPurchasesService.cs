using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IPurchasesService : IBaseReadOnlyService<BaseSearch, PurchasesRes>
    {
    }
}
