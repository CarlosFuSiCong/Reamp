import { useMutation, useQueryClient } from "@tanstack/react-query";
import { authApi, profilesApi } from "@/lib/api";
import { useAuthStore } from "@/lib/stores/auth-store";
import { useRouter } from "next/navigation";
import { UserRole } from "@/types";

function getRedirectPath(role: UserRole): string {
  switch (role) {
    case UserRole.Admin:
      return "/admin/dashboard";
    case UserRole.Client:
      return "/agent/dashboard";
    case UserRole.Staff:
      return "/studio/dashboard";
    default:
      return "/";
  }
}

export function useAuth() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, setUser, logout: logoutStore, isAuthenticated } = useAuthStore();

  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: async (userInfo) => {
      console.log("Login API response:", userInfo);
      const userData = {
        id: userInfo.userId,
        email: userInfo.email,
        role: userInfo.role,
        createdAt: userInfo.createdAtUtc,
        updatedAt: userInfo.updatedAtUtc,
      };
      setUser(userData);
      queryClient.invalidateQueries({ queryKey: ["user"] });
      
      // Redirect based on user role
      const redirectPath = getRedirectPath(userInfo.role);
      console.log("Redirecting to:", redirectPath, "for role:", userInfo.role);
      router.push(redirectPath);
    },
    onError: (error) => {
      console.error("Login mutation error:", error);
    },
  });

  const registerMutation = useMutation({
    mutationFn: authApi.register,
    onSuccess: async (userInfo) => {
      const userData = {
        id: userInfo.userId,
        email: userInfo.email,
        role: userInfo.role,
        createdAt: userInfo.createdAtUtc,
        updatedAt: userInfo.updatedAtUtc,
      };
      setUser(userData);
      queryClient.invalidateQueries({ queryKey: ["user"] });
      
      // Redirect based on user role
      const redirectPath = getRedirectPath(userInfo.role);
      router.push(redirectPath);
    },
  });

  const logoutMutation = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      logoutStore();
      queryClient.clear();
      router.push("/login");
    },
  });

  return {
    user,
    isAuthenticated,
    login: loginMutation.mutateAsync,
    register: registerMutation.mutateAsync,
    logout: logoutMutation.mutateAsync,
    isLoggingIn: loginMutation.isPending,
    isRegistering: registerMutation.isPending,
    isLoggingOut: logoutMutation.isPending,
    loginError: loginMutation.error,
    registerError: registerMutation.error,
  };
}
