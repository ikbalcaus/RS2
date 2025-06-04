using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface INotificationsService
    {
        Task<PagedResult<NotificationsRes>> GetByUserId(BaseSearch search);
        Task<NotificationsRes> MarkAsRead(int notificationId);
        Task<NotificationsRes> Delete(int notificationId);
    }
}
