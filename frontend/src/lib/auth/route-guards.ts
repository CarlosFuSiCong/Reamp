import { UserRole } from "@/types/enums";

export function isAuthenticated(): boolean {
  if (typeof window === "undefined") return false;
  return !!localStorage.getItem("accessToken");
}

export function getUserRole(): UserRole | null {
  if (typeof window === "undefined") return null;
  
  const authStorage = localStorage.getItem("reamp-auth-storage");
  if (!authStorage) return null;
  
  try {
    const parsed = JSON.parse(authStorage);
    return parsed.state?.user?.role || null;
  } catch {
    return null;
  }
}

export function hasRole(role: UserRole): boolean {
  return getUserRole() === role;
}

export function hasAnyRole(roles: UserRole[]): boolean {
  const userRole = getUserRole();
  return userRole !== null && roles.includes(userRole);
}

export function requireAuth(redirectTo = "/login"): boolean {
  if (!isAuthenticated()) {
    if (typeof window !== "undefined") {
      const currentPath = window.location.pathname;
      window.location.href = `${redirectTo}?redirect=${encodeURIComponent(currentPath)}`;
    }
    return false;
  }
  return true;
}

export function requireRole(role: UserRole, fallbackPath?: string): boolean {
  if (!requireAuth()) return false;

  if (!hasRole(role)) {
    if (typeof window !== "undefined") {
      window.location.href = fallbackPath || getRoleDashboard(getUserRole()!);
    }
    return false;
  }

  return true;
}

export function redirectIfAuthenticated(redirectTo?: string): boolean {
  if (isAuthenticated()) {
    if (typeof window !== "undefined") {
      const role = getUserRole();
      window.location.href = redirectTo || getRoleDashboard(role!);
    }
    return true;
  }
  return false;
}

export function getRoleDashboard(role: UserRole): string {
  switch (role) {
    case UserRole.Client:
      return "/agent/dashboard";
    case UserRole.Staff:
      return "/photographer/dashboard";
    case UserRole.Admin:
      return "/admin/dashboard";
    default:
      return "/profile";
  }
}

export function canAccessRoute(pathname: string, role: UserRole): boolean {
  if (pathname.startsWith("/profile")) return true;
  if (pathname.startsWith("/agent")) return role === UserRole.Client;
  if (pathname.startsWith("/photographer")) return role === UserRole.Staff;
  if (pathname.startsWith("/admin")) return role === UserRole.Admin;
  return true;
}

export function decodeToken(token: string): Record<string, unknown> | null {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}

export function isTokenExpired(token: string): boolean {
  const payload = decodeToken(token);
  if (!payload || typeof payload.exp !== "number") return true;
  return payload.exp * 1000 < Date.now();
}

export function clearAuth(): void {
  if (typeof window === "undefined") return;
  localStorage.removeItem("accessToken");
  localStorage.removeItem("reamp-auth-storage");
}

