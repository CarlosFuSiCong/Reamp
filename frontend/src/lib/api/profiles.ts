import apiClient from "@/lib/api-client";
import { UserProfile } from "@/types";

export const profilesApi = {
  async getMe(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>("/api/profiles/me");
    return response.data;
  },

  async update(data: Partial<UserProfile>): Promise<void> {
    await apiClient.put("/api/profiles/me", data);
  },

  async getById(id: string): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>(`/api/profiles/${id}`);
    return response.data;
  },
};
