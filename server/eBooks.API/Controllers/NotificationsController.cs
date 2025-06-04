using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        protected INotificationsService _service;

        public NotificationsController(INotificationsService service)
        {
            _service = service;
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<PagedResult<NotificationsRes>> GetByUserId([FromQuery] BaseSearch search)
        {
            return await _service.GetByUserId(search);
        }

        [Authorize(Policy = "User")]
        [HttpPatch("{notificationId}")]
        public async Task<NotificationsRes> MarkAsRead(int notificationId)
        {
            return await _service.MarkAsRead(notificationId);
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{notificationId}")]
        public async Task<NotificationsRes> Delete(int notificationId)
        {
            return await _service.Delete(notificationId);
        }
    }
}
