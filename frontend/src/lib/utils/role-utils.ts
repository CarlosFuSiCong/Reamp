import { AgencyRole, StudioRole } from "@/types";

export function getAgencyRoleName(role: AgencyRole | undefined | null): string {
  if (role === undefined || role === null) return "Agent";
  
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  
  switch (roleValue) {
    case AgencyRole.Owner:
      return "Owner";
    case AgencyRole.Manager:
      return "Manager";
    case AgencyRole.Agent:
      return "Agent";
    default:
      return "Unknown";
  }
}

export function getStudioRoleName(role: StudioRole | undefined | null): string {
  if (role === undefined || role === null) return "Staff";
  
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  
  switch (roleValue) {
    case StudioRole.Owner:
      return "Owner";
    case StudioRole.Manager:
      return "Manager";
    case StudioRole.Staff:
      return "Staff";
    default:
      return "Unknown";
  }
}

export function canInviteAgencyMembers(role: AgencyRole | undefined | null): boolean {
  if (role === undefined || role === null) return false;
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  return roleValue === AgencyRole.Owner || roleValue === AgencyRole.Manager;
}

export function canInviteStudioMembers(role: StudioRole | undefined | null): boolean {
  if (role === undefined || role === null) return false;
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  return roleValue === StudioRole.Owner || roleValue === StudioRole.Manager;
}

export function canManageAgencyMember(userRole: AgencyRole | undefined | null, targetRole: AgencyRole): boolean {
  if (userRole === undefined || userRole === null) return false;
  const userRoleValue = typeof userRole === 'string' ? parseInt(userRole, 10) : Number(userRole);
  return targetRole !== AgencyRole.Owner && (userRoleValue === AgencyRole.Owner || userRoleValue === AgencyRole.Manager);
}

export function canManageStudioMember(userRole: StudioRole | undefined | null, targetRole: StudioRole): boolean {
  if (userRole === undefined || userRole === null) return false;
  const userRoleValue = typeof userRole === 'string' ? parseInt(userRole, 10) : Number(userRole);
  return targetRole !== StudioRole.Owner && (userRoleValue === StudioRole.Owner || userRoleValue === StudioRole.Manager);
}
