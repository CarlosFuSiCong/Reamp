import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "@/lib/api/listings";
import { ordersApi } from "@/lib/api/orders";
import { ListingStatus, OrderStatus } from "@/types";

export function useDashboardStats() {
  const { data: listingsData, isLoading: loadingListings, error: listingsError } = useQuery({
    queryKey: ["listings", { page: 1, pageSize: 100 }],
    queryFn: () => listingsApi.list({ page: 1, pageSize: 100 }),
  });

  const { data: ordersData, isLoading: loadingOrders, error: ordersError } = useQuery({
    queryKey: ["orders", { page: 1, pageSize: 100 }],
    queryFn: () => ordersApi.list({ page: 1, pageSize: 100 }),
  });

  const listings = listingsData?.items || [];
  const orders = ordersData?.items || [];

  const stats = {
    totalListings: listings.filter(l => l.status === ListingStatus.Active).length,
    activeOrders: orders.filter(o => 
      o.status === OrderStatus.Placed || o.status === OrderStatus.Accepted
    ).length,
    pendingListings: listings.filter(l => l.status === ListingStatus.Pending).length,
    totalClients: 0,
  };

  return {
    stats,
    listings,
    orders,
    isLoading: loadingListings || loadingOrders,
    error: listingsError || ordersError,
  };
}

