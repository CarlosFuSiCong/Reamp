using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Reamp.Application.Common.Services;
using Reamp.Domain.Accounts.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Reamp.Api.Attributes
{
    /// <summary>
    /// Require user to have specified studio role or higher
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireStudioRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly StudioRole _requiredRole;
        private readonly string _studioIdRouteKey;

        public RequireStudioRoleAttribute(StudioRole requiredRole, string studioIdRouteKey = "studioId")
        {
            _requiredRole = requiredRole;
            _studioIdRouteKey = studioIdRouteKey;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
            if (permissionService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Get user ID from claims
            var userIdClaim = context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get studio ID from route
            if (!context.RouteData.Values.TryGetValue(_studioIdRouteKey, out var studioIdObj) ||
                !Guid.TryParse(studioIdObj?.ToString(), out var studioId))
            {
                context.Result = new BadRequestObjectResult(new { error = $"Missing or invalid {_studioIdRouteKey}" });
                return;
            }

            // Check permission
            var hasPermission = await permissionService.HasStudioRoleAsync(userId, studioId, _requiredRole);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
