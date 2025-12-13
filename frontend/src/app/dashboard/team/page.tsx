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

  // Show agency team if user is part of an agency
  if (profile?.agencyId) {
    return <AgencyTeamPage />;
  }

  // Show studio team if user is part of a studio
  if (profile?.studioId) {
    return <StudioTeamPage />;
  }

  // Fallback (shouldn't reach here due to useEffect redirect)
  return <LoadingState message="Redirecting..." />;
}
