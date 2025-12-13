import apiClient from "@/lib/api-client";
import {
  InvitationListDto,
  InvitationDetailDto,
  SendAgencyInvitationDto,
  SendStudioInvitationDto,
} from "@/types";

export const invitationsApi = {
  // General invitation endpoints
  async getMyInvitations(): Promise<InvitationListDto[]> {
    const response = await apiClient.get<InvitationListDto[]>("/api/invitations/me");
    return response.data;
  },

  async getInvitationById(invitationId: string): Promise<InvitationDetailDto> {
    const response = await apiClient.get<InvitationDetailDto>(`/api/invitations/${invitationId}`);
    return response.data;
  },

  async acceptInvitation(invitationId: string): Promise<void> {
    await apiClient.post(`/api/invitations/${invitationId}/accept`);
  },

  async rejectInvitation(invitationId: string): Promise<void> {
    await apiClient.post(`/api/invitations/${invitationId}/reject`);
  },

  async cancelInvitation(invitationId: string): Promise<void> {
    await apiClient.post(`/api/invitations/${invitationId}/cancel`);
  },

  // Agency invitation endpoints
  async sendAgencyInvitation(
    agencyId: string,
    data: SendAgencyInvitationDto
  ): Promise<InvitationDetailDto> {
    const response = await apiClient.post<InvitationDetailDto>(
      `/api/agencies/${agencyId}/invitations`,
      data
    );
    return response.data;
  },

  async getAgencyInvitations(agencyId: string): Promise<InvitationDetailDto[]> {
    const response = await apiClient.get<InvitationDetailDto[]>(
      `/api/agencies/${agencyId}/invitations`
    );
    return response.data;
  },

  // Studio invitation endpoints
  async sendStudioInvitation(
    studioId: string,
    data: SendStudioInvitationDto
  ): Promise<InvitationDetailDto> {
    const response = await apiClient.post<InvitationDetailDto>(
      `/api/studios/${studioId}/invitations`,
      data
    );
    return response.data;
  },

  async getStudioInvitations(studioId: string): Promise<InvitationDetailDto[]> {
    const response = await apiClient.get<InvitationDetailDto[]>(
      `/api/studios/${studioId}/invitations`
    );
    return response.data;
  },
};
