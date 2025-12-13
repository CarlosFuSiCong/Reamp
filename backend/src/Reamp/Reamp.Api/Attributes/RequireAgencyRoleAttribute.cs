using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Reamp.Application.Common.Services;
using Reamp.Domain.Accounts.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Reamp.Api.Attributes
{
    /// <summary>
    /// Require user to have specified agency role or higher
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireAgencyRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly AgencyRole _requiredRole;
        private readonly string _agencyIdRouteKey;

        public RequireAgencyRoleAttribute(AgencyRole requiredRole, string agencyIdRouteKey = "agencyId")
        {
            _requiredRole = requiredRole;
            _agencyIdRouteKey = agencyIdRouteKey;
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

            // Get agency ID from route
            if (!context.RouteData.Values.TryGetValue(_agencyIdRouteKey, out var agencyIdObj) ||
                !Guid.TryParse(agencyIdObj?.ToString(), out var agencyId))
            {
                context.Result = new BadRequestObjectResult(new { error = $"Missing or invalid {_agencyIdRouteKey}" });
                return;
            }

            // Check permission
            var hasPermission = await permissionService.HasAgencyRoleAsync(userId, agencyId, _requiredRole);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
