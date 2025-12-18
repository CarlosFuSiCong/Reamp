import { useMutation, useQueryClient } from "@tanstack/react-query";
import { profilesApi, UpdateProfileDto } from "@/lib/api/profiles";
import { toast } from "sonner";
import { useAuthStore } from "@/lib/stores/auth-store";
import { useRouter } from "next/navigation";

export function useUpdateProfile() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ profileId, data }: { profileId: string; data: UpdateProfileDto }) =>
      profilesApi.update(profileId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["profile"] });
      toast.success("Profile updated successfully");
    },
    onError: (error: unknown) => {
      const err = error as { message?: string };
      toast.error(err?.message || "Failed to update profile");
    },
  });
}

export function useUpdateAvatar() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ profileId, assetId }: { profileId: string; assetId: string }) =>
      profilesApi.updateAvatar(profileId, assetId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["profile"] });
      toast.success("Avatar updated successfully");
    },
    onError: (error: unknown) => {
      const err = error as { message?: string };
      toast.error(err?.message || "Failed to update avatar");
    },
  });
}

export function useChangePassword() {
  const logout = useAuthStore((state) => state.logout);
  const router = useRouter();
  
  return useMutation({
    mutationFn: profilesApi.changePassword,
    onSuccess: () => {
      toast.success("Password changed successfully. Please log in again.");
      setTimeout(() => {
        logout();
        router.push("/login");
      }, 1500);
    },
    onError: (error: unknown) => {
      const err = error as { message?: string };
      toast.error(err?.message || "Failed to change password");
    },
  });
}
