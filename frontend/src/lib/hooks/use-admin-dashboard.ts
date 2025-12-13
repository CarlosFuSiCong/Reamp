/**
 * Admin Dashboard state management hook
 * Consolidates all admin dashboard related state and logic
 */
import { useAdminStats } from "./use-admin-stats";

export interface AdminDashboardState {
  stats: {
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
  };
  activities: Array<{
    id: string;
    type: string;
    title: string;
    description: string;
    timestamp: Date;
  }>;
  isLoading: boolean;
  error: Error | null;
}

const DEFAULT_STATS = {
  totalUsers: 0,
  activeListings: 0,
  totalOrders: 0,
  totalStudios: 0,
  chartData: [],
  alerts: []
};

export function useAdminDashboard(): AdminDashboardState {
  const { stats, activities, isLoading, error } = useAdminStats();

  return {
    stats: error ? DEFAULT_STATS : stats,
    activities: error ? [] : activities,
    isLoading,
    error
  };
}
