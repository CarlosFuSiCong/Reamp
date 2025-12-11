import apiClient from "@/lib/api-client";
import { Agency, AgencyDetail, AgencyBranch, PagedResponse } from "@/types";

export const agenciesApi = {
  async list(params: {
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<Agency>> {
    const response = await apiClient.get<PagedResponse<Agency>>("/api/agencies", {
      params,
    });
    return response.data;
  },

  async getById(id: string): Promise<AgencyDetail> {
    const response = await apiClient.get<AgencyDetail>(`/api/agencies/${id}`);
    return response.data;
  },

  async create(data: Partial<Agency>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/agencies", data);
    return response.data;
  },

  async update(id: string, data: Partial<Agency>): Promise<void> {
    await apiClient.put(`/api/agencies/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/agencies/${id}`);
  },

  async getBranches(agencyId: string): Promise<AgencyBranch[]> {
    const response = await apiClient.get<AgencyBranch[]>(`/api/agencies/${agencyId}/branches`);
    return response.data;
  },

  async createBranch(agencyId: string, data: Partial<AgencyBranch>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>(
      `/api/agencies/${agencyId}/branches`,
      data
    );
    return response.data;
  },
};
