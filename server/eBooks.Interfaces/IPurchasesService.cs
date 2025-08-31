using eBooks.Models.Responses;
using eBooks.Models.Search;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IPurchasesService : IBaseReadOnlyService<PurchasesSearch, PurchasesRes>
    {
        Task<PagedResult<PurchasesRes>> GetAllByPublisherId(int publisherId, BaseSearch search);
    }
}
