import { useQuery } from "@tanstack/react-query";
import { adminApi } from "@/lib/api/admin";

export function useAdminStats() {
  const { data, isLoading, error } = useQuery({
    queryKey: ["admin-stats"],
    queryFn: () => adminApi.getStats(),
    refetchInterval: 30000,
  });

  return {
    stats: data?.stats || {
      totalUsers: 0,
      activeListings: 0,
      totalOrders: 0,
      totalStudios: 0,
      chartData: [],
      alerts: [],
    },
    activities: data?.activities || [],
    isLoading,
    error,
  };
}

export function useUsers() {
  return useQuery({
    queryKey: ["admin-users"],
    queryFn: () => adminApi.getUsers(),
  });
}
