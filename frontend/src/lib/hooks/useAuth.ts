import { useMutation, useQueryClient } from "@tanstack/react-query";
import { authApi, profilesApi } from "@/lib/api";
import { useAuthStore } from "@/lib/stores/auth-store";
import { useRouter } from "next/navigation";
import { UserRole } from "@/types";

export function useAuth() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, setUser, logout: logoutStore, isAuthenticated } = useAuthStore();

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
      router.push("/profile");
    },
    onError: () => {
      router.push("/");
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
      router.push("/profile");
    },
    onError: () => {
      router.push("/");
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
