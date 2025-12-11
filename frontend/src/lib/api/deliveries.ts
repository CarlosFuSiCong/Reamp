import apiClient from "@/lib/api-client";
import { DeliveryPackage, PagedResponse } from "@/types";

export const deliveriesApi = {
  async list(params: {
    orderId?: string;
    listingId?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<DeliveryPackage>> {
    const response = await apiClient.get<PagedResponse<DeliveryPackage>>("/api/deliveries", {
      params,
    });
    return response.data;
  },

  async getById(id: string): Promise<DeliveryPackage> {
    const response = await apiClient.get<DeliveryPackage>(`/api/deliveries/${id}`);
    return response.data;
  },

  async create(data: Partial<DeliveryPackage>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/deliveries", data);
    return response.data;
  },

  async update(id: string, data: Partial<DeliveryPackage>): Promise<void> {
    await apiClient.put(`/api/deliveries/${id}`, data);
  },

  async publish(id: string): Promise<void> {
    await apiClient.post(`/api/deliveries/${id}/publish`);
  },

  async revoke(id: string): Promise<void> {
    await apiClient.post(`/api/deliveries/${id}/revoke`);
  },
};
