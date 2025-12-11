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
    onSuccess: async (tokenResponse) => {
      try {
        // Store access token in localStorage for API requests
        if (typeof window !== "undefined") {
          localStorage.setItem("accessToken", tokenResponse.accessToken);
        }

        const profile = await profilesApi.getMe();
        const userData = {
          id: profile.applicationUserId,
          email: "",
          role: profile.role,
          createdAt: profile.createdAtUtc,
          updatedAt: profile.updatedAtUtc,
        };
        setUser(userData);
        queryClient.invalidateQueries({ queryKey: ["user"] });

        switch (profile.role) {
          case UserRole.Client:
            router.push("/client/dashboard");
            break;
          case UserRole.Staff:
            router.push("/staff/dashboard");
            break;
          case UserRole.Admin:
            router.push("/admin/dashboard");
            break;
          default:
            router.push("/");
        }
      } catch (error) {
        console.error("Failed to fetch user profile:", error);
        router.push("/");
      }
    },
  });

  const registerMutation = useMutation({
    mutationFn: authApi.register,
    onSuccess: async (tokenResponse) => {
      try {
        // Store access token in localStorage for API requests
        if (typeof window !== "undefined") {
          localStorage.setItem("accessToken", tokenResponse.accessToken);
        }

        // Fetch user profile
        const profile = await profilesApi.getMe();
        const userData = {
          id: profile.applicationUserId,
          email: "",
          role: profile.role,
          createdAt: profile.createdAtUtc,
          updatedAt: profile.updatedAtUtc,
        };
        setUser(userData);
        queryClient.invalidateQueries({ queryKey: ["user"] });

        // Redirect based on role
        switch (profile.role) {
          case UserRole.Client:
            router.push("/client/dashboard");
            break;
          case UserRole.Staff:
            router.push("/staff/dashboard");
            break;
          case UserRole.Admin:
            router.push("/admin/dashboard");
            break;
          default:
            router.push("/");
        }
      } catch (error) {
        console.error("Failed to fetch user profile:", error);
        router.push("/");
      }
    },
    onError: (error) => {
      console.error("Registration failed:", error);
    },
  });

  const logoutMutation = useMutation({
    mutationFn: authApi.logout,
    onSuccess: () => {
      // Clear access token
      if (typeof window !== "undefined") {
        localStorage.removeItem("accessToken");
      }
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
