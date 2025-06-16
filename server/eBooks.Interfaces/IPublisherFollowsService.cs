using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IPublisherFollowsService
    {
        Task<PagedResult<PublisherFollowsRes>> GetPaged();
        Task<PublisherFollowsRes> GetByPublisherId(int publisherId);
        Task<PublisherFollowsRes> Post(int publisherId);
        Task<PublisherFollowsRes> Delete(int publisherId);
    }
}
