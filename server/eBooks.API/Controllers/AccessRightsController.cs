using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessRightsController : BaseUserContextController<object, AccessRightsRes>
    {
        public AccessRightsController(IAccessRightsService service)
            : base(service)
        {
        }
    }
}
