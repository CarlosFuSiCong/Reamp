import apiClient from "@/lib/api-client";
import { Studio, PagedResponse } from "@/types";

export const studiosApi = {
  async list(params: {
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<Studio>> {
    const response = await apiClient.get<PagedResponse<Studio>>("/api/accounts/studios", {
      params,
    });
    return response.data;
  },

  async getById(id: string): Promise<Studio> {
    const response = await apiClient.get<Studio>(`/api/accounts/studios/${id}`);
    return response.data;
  },

  async create(data: Partial<Studio>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/accounts/studios", data);
    return response.data;
  },

  async update(id: string, data: Partial<Studio>): Promise<void> {
    await apiClient.put(`/api/accounts/studios/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/accounts/studios/${id}`);
  },
};
