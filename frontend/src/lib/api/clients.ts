import apiClient from "@/lib/api-client";
import { Client, PagedResponse } from "@/types";

export const clientsApi = {
  async list(params: {
    agencyId?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<Client>> {
    const response = await apiClient.get<PagedResponse<Client>>("/api/clients", {
      params,
    });
    return response.data;
  },

  async getById(id: string): Promise<Client> {
    const response = await apiClient.get<Client>(`/api/clients/${id}`);
    return response.data;
  },

  async create(data: Partial<Client>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/clients", data);
    return response.data;
  },

  async update(id: string, data: Partial<Client>): Promise<void> {
    await apiClient.put(`/api/clients/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/clients/${id}`);
  },
};
