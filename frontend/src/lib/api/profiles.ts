import apiClient from "@/lib/api-client";
import { UserProfile } from "@/types";

export interface UpdateProfileDto {
  firstName: string;
  lastName?: string;  // Optional to match backend domain model
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export const profilesApi = {
  async getMe(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>("/api/profiles/me");
    return response.data;
  },

  async update(profileId: string, data: UpdateProfileDto): Promise<void> {
    await apiClient.put(`/api/profiles/${profileId}`, data);
  },

  async getById(id: string): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>(`/api/profiles/${id}`);
    return response.data;
  },

  async updateAvatar(profileId: string, assetId: string): Promise<void> {
    await apiClient.put(`/api/profiles/${profileId}/avatar`, { avatarAssetId: assetId });
  },

  async changePassword(data: ChangePasswordDto): Promise<void> {
    await apiClient.put("/api/auth/password", data);
  },
};
