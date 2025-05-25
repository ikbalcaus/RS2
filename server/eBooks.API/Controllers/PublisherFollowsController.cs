using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("[controller]")]
    public class PublisherFollowsController : ControllerBase
    {
        protected IPublisherFollowsService _service;

        public PublisherFollowsController(IPublisherFollowsService service)
        {
            _service = service;
        }

        [Authorize(Policy = "User")]
        [HttpGet("{userId}")]
        public async Task<PagedResult<PublisherFollowsRes>> GetByUserId(int userId)
        {
            return await _service.GetByUserId(userId);
        }

        [Authorize(Policy = "User")]
        [HttpPost("{publisherId}")]
        public async Task<PublisherFollowsRes> Post(int publisherId)
        {
            return await _service.Post(publisherId);
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{publisherId}")]
        public async Task<PublisherFollowsRes> Delete(int publisherId)
        {
            return await _service.Delete(publisherId);
        }
    }
}
