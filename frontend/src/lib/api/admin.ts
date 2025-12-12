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
  type: string;
  description: string;
  user?: string;
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
};
