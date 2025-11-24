namespace Reamp.Application.Authentication
{
    // Authorization policy names
    public static class AuthPolicies
    {
        // Role-based policies
        public const string RequireAdminRole = "RequireAdminRole";
        public const string RequireStaffRole = "RequireStaffRole";
        public const string RequireClientRole = "RequireClientRole";
        public const string RequireUserRole = "RequireUserRole";

        // Combined policies
        public const string RequireStaffOrAdmin = "RequireStaffOrAdmin";
        public const string RequireClientOrAdmin = "RequireClientOrAdmin";
    }
}




