import { useMutation, useQueryClient } from "@tanstack/react-query";
import { authApi, profilesApi } from "@/lib/api";
import { useAuthStore } from "@/lib/stores/auth-store";
import { useRouter } from "next/navigation";
import { UserRole } from "@/types";
import { toast } from "sonner";

function getRedirectPath(role: UserRole): string {
  switch (role) {
    case UserRole.Admin:
      return "/admin";
    case UserRole.Client:
      return "/";
    case UserRole.Agent:
      return "/dashboard/agency";
    case UserRole.Staff:
      return "/apply";
    default:
      return "/";
  }
}

export function useAuth() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, setUser, logout: logoutStore, isAuthenticated, isLoading } = useAuthStore();

  const loginMutation = useMutation({
    mutationFn: authApi.login,
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
      
      toast.success("Login successful", {
        description: `Welcome back, ${userInfo.email}!`,
      });
      
      const redirectPath = getRedirectPath(userInfo.role);
      window.location.href = redirectPath;
    },
    onError: (error: any) => {
      toast.error("Login failed", {
        description: error?.message || "Please check your credentials and try again",
      });
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
      
      toast.success("Registration successful", {
        description: `Welcome to Reamp, ${userInfo.email}!`,
      });
      
      const redirectPath = getRedirectPath(userInfo.role);
      window.location.href = redirectPath;
    },
    onError: (error: any) => {
      toast.error("Registration failed", {
        description: error?.message || "Please check your information and try again",
      });
    },
  });

  const logoutMutation = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      logoutStore();
      queryClient.clear();
      toast.success("Logged out successfully", {
        description: "See you next time!",
      });
      router.push("/login");
    },
    onError: () => {
      toast.error("Logout failed", {
        description: "Please try again",
      });
    },
  });

  return {
    user,
    isAuthenticated,
    isLoading,
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
