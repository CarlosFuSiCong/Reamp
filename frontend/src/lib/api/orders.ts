import apiClient from "@/lib/api-client";
import { PagedResponse, ShootOrder } from "@/types";

interface BackendPagedResponse<T> {
  items: T[];
  totalCount?: number;
  total?: number;
  page: number;
  pageSize: number;
}

export const ordersApi = {
  async list(params: {
    agencyId?: string;
    studioId?: string;
    listingId?: string;
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<ShootOrder>> {
    const response = await apiClient.get<BackendPagedResponse<ShootOrder>>("/api/orders", {
      params,
    });
    // Map totalCount to total for frontend compatibility
    return {
      ...response.data,
      total: response.data.totalCount || response.data.total || 0,
    };
  },

  async getById(id: string): Promise<ShootOrder> {
    const response = await apiClient.get<ShootOrder>(`/api/orders/${id}`);
    return response.data;
  },

  async create(data: {
    agencyId?: string; // Optional - will be auto-populated by backend from current user
    studioId?: string; // Optional - if not provided, order is published to marketplace
    listingId: string;
    currency?: string;
  }): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/orders", data);
    return response.data;
  },

  async addTask(
    orderId: string,
    task: {
      type: number;
      notes?: string;
      price?: number;
    }
  ): Promise<void> {
    await apiClient.post(`/api/orders/${orderId}/tasks`, task);
  },

  async cancel(id: string, reason?: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/cancel`, { reason });
  },

  async accept(id: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/accept`);
  },

  async start(id: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/start`);
  },

  async complete(id: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/complete`);
  },

  // Photographer-specific endpoints
  async getAvailableOrders(params: {
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<ShootOrder>> {
    const response = await apiClient.get<BackendPagedResponse<ShootOrder>>(
      "/api/orders/available",
      {
        params,
      }
    );
    // Map totalCount to total for frontend compatibility
    return {
      ...response.data,
      total: response.data.totalCount || response.data.total || 0,
    };
  },

  async getMyOrders(params: {
    status?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<ShootOrder>> {
    const response = await apiClient.get<BackendPagedResponse<ShootOrder>>(
      "/api/orders/my-orders",
      {
        params,
      }
    );
    // Map totalCount to total for frontend compatibility
    return {
      ...response.data,
      total: response.data.totalCount || response.data.total || 0,
    };
  },

  async acceptAsPhotographer(id: string): Promise<void> {
    await apiClient.post(`/api/orders/${id}/accept-photographer`);
  },
};
