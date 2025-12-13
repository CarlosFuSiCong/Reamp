"use client";

import { ReactNode } from "react";
import { useAuth } from "@/lib/hooks/use-auth";
import { UserRole } from "@/types";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { LoadingState } from "@/components/shared/loading-state";

export default function AdminLayout({ children }: { children: ReactNode }) {
  const { user, isAuthenticated } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!user || user.role !== UserRole.Admin) {
      router.push("/profile");
    }
  }, [user, router]);

  if (!user || user.role !== UserRole.Admin) {
    return null;
  }

  return <>{children}</>;
}
