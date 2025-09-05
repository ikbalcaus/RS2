using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IOverviewService
    {
        Task<OverviewRes> GetAllCount();
    }
}
