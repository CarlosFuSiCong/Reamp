import apiClient from "../api-client";
import { StaffSkills } from "@/types/enums";

export interface StaffDetailDto {
  id: string;
  userProfileId: string;
  studioId: string;
  studioName?: string;
  role: number; // StudioRole
  skills: StaffSkills;
  createdAtUtc: string;
  updatedAtUtc: string;
  firstName: string;
  lastName: string;
  displayName: string;
}

export interface UpdateStaffSkillsDto {
  skills: StaffSkills;
}

export const staffApi = {
  // Get staff by ID
  async getById(id: string): Promise<StaffDetailDto> {
    const response = await apiClient.get<StaffDetailDto>(`/api/staff/${id}`);
    return response.data;
  },

  // Get staff by user profile ID
  async getByUserProfileId(userProfileId: string): Promise<StaffDetailDto | null> {
    try {
      const response = await apiClient.get<StaffDetailDto>(`/api/staff/profile/${userProfileId}`);
      return response.data;
    } catch (error: unknown) {
      if (error.status === 404) return null;
      throw error;
    }
  },

  // Update staff skills
  async updateSkills(staffId: string, skills: StaffSkills): Promise<StaffDetailDto> {
    const response = await apiClient.put<StaffDetailDto>(`/api/staff/${staffId}/skills`, {
      skills,
    });
    return response.data;
  },
};
