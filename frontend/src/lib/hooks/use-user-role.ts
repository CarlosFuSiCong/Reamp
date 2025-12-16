import { useMemo } from "react";
import { UserProfile } from "@/types";

interface UseUserRoleReturn {
  isAgent: boolean;
  isStudio: boolean;
  isAdmin: boolean;
}

/**
 * Custom hook to determine user role
 */
export function useUserRole(profile: UserProfile | null | undefined): UseUserRoleReturn {
  return useMemo(() => {
    return {
      isAgent: profile?.agencyRole !== undefined && profile?.agencyRole !== null,
      isStudio: profile?.studioRole !== undefined && profile?.studioRole !== null,
      isAdmin: profile?.role === 4, // UserRole.Admin
    };
  }, [profile]);
}
