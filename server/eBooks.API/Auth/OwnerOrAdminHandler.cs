using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eBooks.API.Auth
{
    public class OwnerOrAdminRequirement : IAuthorizationRequirement { }

    public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement, int resourceOwnerId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (context.User.IsInRole("Admin") || (userIdClaim != null && userIdClaim == resourceOwnerId.ToString()))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
