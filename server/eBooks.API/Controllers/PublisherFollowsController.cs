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
        [HttpGet]
        public async Task<PagedResult<PublisherFollowsRes>> GetPaged()
        {
            return await _service.GetPaged();
        }

        [Authorize(Policy = "User")]
        [HttpGet("{publisherId}")]
        public async Task<PublisherFollowsRes> GetByPublisherId(int publisherId)
        {
            return await _service.GetByPublisherId(publisherId);
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
