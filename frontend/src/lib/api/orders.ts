import apiClient from "@/lib/api-client";
import { PagedResponse, ShootOrder } from "@/types";

export const ordersApi = {
  async list(params: {
    agencyId?: string;
    studioId?: string;
    listingId?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<ShootOrder>> {
    const response = await apiClient.get<PagedResponse<ShootOrder>>("/api/orders", {
      params,
    });
    return response.data;
  },

  async getById(id: string): Promise<ShootOrder> {
    const response = await apiClient.get<ShootOrder>(`/api/orders/${id}`);
    return response.data;
  },

  async create(data: {
    agencyId: string;
    studioId: string;
    listingId: string;
    currency?: string;
  }): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/orders", data);
    return response.data;
  },

  async cancel(id: string, reason?: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/cancel`, { reason });
  },

  async accept(id: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/accept`);
  },

  async complete(id: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/complete`);
  },
};
