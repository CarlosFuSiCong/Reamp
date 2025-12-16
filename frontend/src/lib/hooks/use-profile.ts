import { useMutation, useQueryClient } from "@tanstack/react-query";
import { profilesApi, UpdateProfileDto } from "@/lib/api/profiles";
import { toast } from "sonner";

export function useUpdateProfile() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ profileId, data }: { profileId: string; data: UpdateProfileDto }) =>
      profilesApi.update(profileId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["profile"] });
      toast.success("Profile updated successfully");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to update profile");
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
    onError: (error: any) => {
      toast.error(error?.message || "Failed to update avatar");
    },
  });
}

export function useChangePassword() {
  return useMutation({
    mutationFn: profilesApi.changePassword,
    onSuccess: () => {
      toast.success("Password changed successfully");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to change password");
    },
  });
}
