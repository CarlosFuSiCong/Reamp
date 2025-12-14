import apiClient from "@/lib/api-client";

export interface AdminStats {
  totalUsers: number;
  activeListings: number;
  totalOrders: number;
  totalStudios: number;
  chartData: Array<{
    date: string;
    orders: number;
    listings: number;
  }>;
  alerts: Array<{
    title: string;
    message: string;
  }>;
}

export interface Activity {
  id: string;
  type: "order" | "listing" | "message" | "other";
  title: string;
  description: string;
  timestamp: string;
}

export interface AdminUser {
  id: string;
  email: string;
  displayName: string;
  role: number;
  status: number;
  createdAtUtc: string;
}

export interface AgencySummary {
  id: string;
  name: string;
  slug: string;
  contactEmail: string;
  contactPhone: string;
  createdBy: string;
  createdAtUtc: string;
}

export interface StudioSummary {
  id: string;
  name: string;
  slug: string;
  contactEmail: string;
  contactPhone: string;
  createdBy: string;
  createdAtUtc: string;
}

export interface CreateAgencyDto {
  name: string;
  description?: string;
  contactEmail: string;
  contactPhone: string;
  ownerUserId: string;
}

export interface CreateStudioDto {
  name: string;
  description?: string;
  contactEmail: string;
  contactPhone: string;
  ownerUserId: string;
}

export const adminApi = {
  async getStats(): Promise<{ stats: AdminStats; activities: Activity[] }> {
    const response = await apiClient.get("/api/admin/stats");
    return response.data;
  },

  async getUsers(): Promise<AdminUser[]> {
    const response = await apiClient.get("/api/admin/users");
    return response.data;
  },

  async updateUserStatus(userId: string, status: number): Promise<void> {
    await apiClient.put(`/api/admin/users/${userId}/status`, { status });
  },

  async updateUserRole(userId: string, role: number): Promise<void> {
    await apiClient.put(`/api/admin/users/${userId}/role`, { role });
  },

  async createAgency(dto: CreateAgencyDto): Promise<AgencySummary> {
    const response = await apiClient.post("/api/admin/agencies", dto);
    return response.data;
  },

  async getAgencies(): Promise<AgencySummary[]> {
    const response = await apiClient.get("/api/admin/agencies");
    return response.data;
  },

  async createStudio(dto: CreateStudioDto): Promise<StudioSummary> {
    const response = await apiClient.post("/api/admin/studios", dto);
    return response.data;
  },

  async getStudios(): Promise<StudioSummary[]> {
    const response = await apiClient.get("/api/admin/studios");
    return response.data;
  },
};
