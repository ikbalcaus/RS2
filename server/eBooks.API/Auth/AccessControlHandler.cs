using System.Security.Claims;
using eBooks.Models.Exceptions;

namespace eBooks.API.Auth
{
    public class AccessControlHandler
    {
        protected IHttpContextAccessor _httpContextAccessor;

        public AccessControlHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CheckIsOwner(int id)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new ExceptionForbidden("Only owner can use this action");
            if (int.Parse(userIdClaim) != id)
                throw new ExceptionForbidden("Only owner can use this action");
        }

        public async Task CheckIsOwnerOrAdmin(int id)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                throw new ExceptionForbidden("Only owner or admin can use this action");
            int currentUserId = int.Parse(userIdClaim);
            if (roleClaim != "Admin" && currentUserId != id)
                throw new ExceptionForbidden("Only owner or admin can use this action");
        }
    }
}
