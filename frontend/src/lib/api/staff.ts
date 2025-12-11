import apiClient from "@/lib/api-client";
import { Staff, PagedResponse } from "@/types";

export const staffApi = {
  async list(params: {
    studioId?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<Staff>> {
    const response = await apiClient.get<PagedResponse<Staff>>("/api/staff", { params });
    return response.data;
  },

  async getById(id: string): Promise<Staff> {
    const response = await apiClient.get<Staff>(`/api/staff/${id}`);
    return response.data;
  },

  async create(data: Partial<Staff>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/staff", data);
    return response.data;
  },

  async update(id: string, data: Partial<Staff>): Promise<void> {
    await apiClient.put(`/api/staff/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/staff/${id}`);
  },
};
