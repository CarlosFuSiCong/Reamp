import apiClient from "@/lib/api-client";
import {
  AgencyMemberDto,
  StudioMemberDto,
  UpdateAgencyMemberRoleDto,
  UpdateStudioMemberRoleDto,
} from "@/types";

export const membersApi = {
  // Agency member endpoints
  async getAgencyMembers(agencyId: string): Promise<AgencyMemberDto[]> {
    const response = await apiClient.get<AgencyMemberDto[]>(
      `/api/agencies/${agencyId}/members`
    );
    return response.data;
  },

  async updateAgencyMemberRole(
    agencyId: string,
    memberId: string,
    data: UpdateAgencyMemberRoleDto
  ): Promise<AgencyMemberDto> {
    const response = await apiClient.put<AgencyMemberDto>(
      `/api/agencies/${agencyId}/members/${memberId}/role`,
      data
    );
    return response.data;
  },

  async removeAgencyMember(agencyId: string, memberId: string): Promise<void> {
    await apiClient.delete(`/api/agencies/${agencyId}/members/${memberId}`);
  },

  // Studio member endpoints
  async getStudioMembers(studioId: string): Promise<StudioMemberDto[]> {
    const response = await apiClient.get<StudioMemberDto[]>(
      `/api/studios/${studioId}/members`
    );
    return response.data;
  },

  async updateStudioMemberRole(
    studioId: string,
    memberId: string,
    data: UpdateStudioMemberRoleDto
  ): Promise<StudioMemberDto> {
    const response = await apiClient.put<StudioMemberDto>(
      `/api/studios/${studioId}/members/${memberId}/role`,
      data
    );
    return response.data;
  },

  async removeStudioMember(studioId: string, memberId: string): Promise<void> {
    await apiClient.delete(`/api/studios/${studioId}/members/${memberId}`);
  },
};
