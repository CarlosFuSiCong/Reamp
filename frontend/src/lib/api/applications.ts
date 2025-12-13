import apiClient from "@/lib/api-client";
import {
  SubmitAgencyApplicationDto,
  SubmitStudioApplicationDto,
  ReviewApplicationDto,
  ApplicationListDto,
  ApplicationDetailDto,
  ApplicationStatus,
  ApplicationType,
} from "@/types";
import { PagedResponse } from "@/types/api";

export const applicationsApi = {
  async submitAgencyApplication(data: SubmitAgencyApplicationDto): Promise<string> {
    const response = await apiClient.post<string>("/api/applications/agency", data);
    return response.data;
  },

  async submitStudioApplication(data: SubmitStudioApplicationDto): Promise<string> {
    const response = await apiClient.post<string>("/api/applications/studio", data);
    return response.data;
  },

  async getApplications(
    page: number = 1,
    pageSize: number = 20,
    status?: ApplicationStatus,
    type?: ApplicationType
  ): Promise<PagedResponse<ApplicationListDto>> {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });
    if (status !== undefined) params.append("status", status.toString());
    if (type !== undefined) params.append("type", type.toString());

    const response = await apiClient.get<PagedResponse<ApplicationListDto>>(
      `/api/applications?${params.toString()}`
    );
    return response.data;
  },

  async getMyApplications(): Promise<ApplicationListDto[]> {
    const response = await apiClient.get<ApplicationListDto[]>("/api/applications/my");
    return response.data;
  },

  async getApplicationDetail(id: string): Promise<ApplicationDetailDto> {
    const response = await apiClient.get<ApplicationDetailDto>(`/api/applications/${id}`);
    return response.data;
  },

  async reviewApplication(id: string, data: ReviewApplicationDto): Promise<void> {
    await apiClient.post(`/api/applications/${id}/review`, data);
  },

  async cancelApplication(id: string): Promise<void> {
    await apiClient.post(`/api/applications/${id}/cancel`);
  },
};
