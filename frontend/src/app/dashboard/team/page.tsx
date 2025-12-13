"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useProfile } from "@/lib/hooks";
import { LoadingState } from "@/components/shared";
import AgencyTeamPage from "./agency";
import StudioTeamPage from "./studio";

export default function TeamPage() {
  const router = useRouter();
  const { user: profile, isLoading } = useProfile();

  useEffect(() => {
    // If user has no agency or studio role, redirect to profile
    // Use role checks (not ID checks) to be consistent with dashboard layout logic
    if (!isLoading && profile && 
        (profile.agencyRole === undefined || profile.agencyRole === null) &&
        (profile.studioRole === undefined || profile.studioRole === null)) {
      router.replace("/dashboard/profile");
    }
  }, [isLoading, profile, router]);

  if (isLoading) {
    return <LoadingState message="Loading team..." />;
  }

  // For users with both agency and studio roles, prioritize agency
  // TODO: Consider adding a tab/toggle UI to switch between agency and studio teams
  const hasAgencyRole = profile?.agencyId && (profile?.agencyRole !== undefined && profile?.agencyRole !== null);
  const hasStudioRole = profile?.studioId && (profile?.studioRole !== undefined && profile?.studioRole !== null);

  // Show agency team if user has agency role
  if (hasAgencyRole) {
    return <AgencyTeamPage />;
  }

  // Show studio team if user has studio role (and no agency role)
  if (hasStudioRole) {
    return <StudioTeamPage />;
  }

  // Fallback (shouldn't reach here due to useEffect redirect)
  return <LoadingState message="Redirecting..." />;
}
