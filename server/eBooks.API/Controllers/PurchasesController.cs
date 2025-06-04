using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PurchasesController : BaseReadOnlyController<BaseSearch, PurchasesRes>
    {
        public PurchasesController(IPurchasesService service)
            : base(service)
        {
        }

        [Authorize(Policy = "Admin")]
        public override async Task<PagedResult<PurchasesRes>> GetPaged([FromQuery] BaseSearch search)
        {
            return await base.GetPaged(search);
        }

        [Authorize(Policy = "Admin")]
        public override async Task<PurchasesRes> GetById(int id)
        {
            return await base.GetById(id);
        }
    }
}
