"use client";

import { useAuth, useProfile } from "@/lib/hooks";
import { redirect } from "next/navigation";

export default function DashboardPage() {
  const { user } = useAuth();
  const { user: profile, isLoading } = useProfile();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Redirect to profile page
  redirect("/dashboard/profile");
}
