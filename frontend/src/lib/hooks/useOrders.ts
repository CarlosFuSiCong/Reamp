import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersApi } from "@/lib/api";

export function useOrders(params?: {
  agencyId?: string;
  studioId?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}) {
  const { data, isLoading, error, refetch } = useQuery({
    queryKey: ["orders", params],
    queryFn: () => ordersApi.list(params || {}),
    staleTime: 1 * 60 * 1000,
  });

  return {
    orders: data?.items || [],
    total: data?.total || 0,
    page: data?.page || 1,
    pageSize: data?.pageSize || 20,
    isLoading,
    error,
    refetch,
  };
}

export function useOrder(id: string) {
  const {
    data: order,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["order", id],
    queryFn: () => ordersApi.getById(id),
    enabled: !!id,
    staleTime: 1 * 60 * 1000,
  });

  return {
    order,
    isLoading,
    error,
  };
}

export function useCreateOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ordersApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
    },
  });
}

export function useCancelOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, reason }: { id: string; reason?: string }) => ordersApi.cancel(id, reason),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      queryClient.invalidateQueries({ queryKey: ["order", variables.id] });
    },
  });
}
