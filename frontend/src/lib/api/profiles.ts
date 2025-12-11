import apiClient from "@/lib/api-client";
import { UserProfile } from "@/types";

export interface UpdateProfileDto {
  firstName?: string;
  lastName?: string;
  displayName?: string;
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

  async update(data: UpdateProfileDto): Promise<void> {
    await apiClient.put("/api/profiles/me", data);
  },

  async getById(id: string): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>(`/api/profiles/${id}`);
    return response.data;
  },

  async updateAvatar(id: string, file: File): Promise<void> {
    const formData = new FormData();
    formData.append("file", file);
    await apiClient.put(`/api/profiles/${id}/avatar`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  async changePassword(data: ChangePasswordDto): Promise<void> {
    await apiClient.put("/api/auth/change-password", data);
  },
};
