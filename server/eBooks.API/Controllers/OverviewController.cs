using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eBooks.Interfaces;
using eBooks.Models.Responses;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OverviewController : ControllerBase
    {
        protected IOverviewService _service;

        public OverviewController(IOverviewService service)
        {
            _service = service;
        }

        [Authorize(Policy = "Moderator")]
        [HttpGet]
        public async Task<OverviewRes> GetAllCount()
        {
            return await _service.GetAllCount();
        }
    }
}
