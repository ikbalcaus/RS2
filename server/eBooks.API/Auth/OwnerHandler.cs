using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eBooks.API.Auth
{
    public class OwnerRequirement : IAuthorizationRequirement { }

    public class OwnerHandler : AuthorizationHandler<OwnerOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement, int resourceOwnerId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && userIdClaim == resourceOwnerId.ToString())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
