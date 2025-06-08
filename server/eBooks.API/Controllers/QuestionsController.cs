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
    public class QuestionsController : BaseReadOnlyController<QuestionsSearch, QuestionsRes>
    {
        protected new IQuestionsService _service;

        public QuestionsController(IQuestionsService service)
            : base(service)
        {
            _service = service;
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<PagedResult<QuestionsRes>> GetPaged([FromQuery] QuestionsSearch search)
        {
            return await base.GetPaged(search);
        }

        [Authorize(Policy = "Moderator")]
        public override async Task<QuestionsRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        [HttpPost]
        public async Task<QuestionsRes> Post(QuestionsReq req)
        {
            return await _service.Post(req);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPatch("{id}")]
        public async Task<QuestionsRes> Patch(int id, QuestionsReq req)
        {
            return await _service.Patch(id, req);
        }
    }
}
