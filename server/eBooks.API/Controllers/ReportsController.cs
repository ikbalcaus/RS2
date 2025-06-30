using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : BaseUserContextController<ReportsSearch, ReportsReq, ReportsRes>
    {
        protected new IReportsService _service;

        public ReportsController(IReportsService service)
            : base(service)
        {
            _service = service;
        }

        [Authorize(Policy = "Moderator")]
        [HttpDelete("{userId}/{bookId}")]
        public async Task<ReportsRes> AdminDelete(int userId, int bookId)
        {
            return await _service.AdminDelete(userId, bookId);
        }
    }
}
