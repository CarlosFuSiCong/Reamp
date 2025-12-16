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
    if (
      !isLoading &&
      profile &&
      (profile.agencyRole === undefined || profile.agencyRole === null) &&
      (profile.studioRole === undefined || profile.studioRole === null)
    ) {
      router.replace("/dashboard/profile");
    }
  }, [isLoading, profile, router]);

  if (isLoading) {
    return <LoadingState message="Loading team..." />;
  }

  // Prioritize agency over studio for dual-role users
  const hasAgencyRole =
    profile?.agencyId && profile?.agencyRole !== undefined && profile?.agencyRole !== null;
  const hasStudioRole =
    profile?.studioId && profile?.studioRole !== undefined && profile?.studioRole !== null;

  if (hasAgencyRole) {
    return <AgencyTeamPage />;
  }

  if (hasStudioRole) {
    return <StudioTeamPage />;
  }

  return <LoadingState message="Redirecting..." />;
}
