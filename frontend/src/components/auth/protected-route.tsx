"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/lib/stores/auth-store";
import { UserRole } from "@/types/enums";
import { isAuthenticated, getRoleDashboard } from "@/lib/auth/route-guards";

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: UserRole[];
}

export function ProtectedRoute({ children, requiredRoles }: ProtectedRouteProps) {
  const router = useRouter();
  const { isAuthenticated: isAuth, user } = useAuthStore();

  useEffect(() => {
    if (!isAuth && !isAuthenticated()) {
      router.push("/login");
      return;
    }

    if (requiredRoles?.length && user && !requiredRoles.includes(user.role)) {
      router.push(getRoleDashboard(user.role));
    }
  }, [isAuth, user, requiredRoles, router]);

  if (!isAuth || !user) return null;
  if (requiredRoles?.length && !requiredRoles.includes(user.role)) return null;

  return <>{children}</>;
}
