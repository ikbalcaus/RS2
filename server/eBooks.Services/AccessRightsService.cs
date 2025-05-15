using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace eBooks.Services
{
    public class AccessRightsService : BaseUserContextService<AccessRight, AccessRightsRes>, IAccessRightsService
    {
        public AccessRightsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor)
        {
        }
    }
}
