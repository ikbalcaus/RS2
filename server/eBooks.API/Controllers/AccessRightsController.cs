using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessRightsController : BaseUserContextController<AccessRightsRes>
    {
        public AccessRightsController(IAccessRightsService service)
            : base(service)
        {
        }
    }
}
