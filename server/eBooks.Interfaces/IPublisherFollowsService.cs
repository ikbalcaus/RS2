using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IPublisherFollowsService
    {
        Task<PagedResult<PublisherFollowsRes>> GetByUserId(int userId);
        Task<PublisherFollowsRes> Post(int publisherId);
        Task<PublisherFollowsRes> Delete(int publisherId);
    }
}
