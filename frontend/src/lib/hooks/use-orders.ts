import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersApi } from "@/lib/api";
import { ShootOrder, PagedResponse, OrderStatus } from "@/types";
import { toast } from "sonner";

export function useOrders(params?: {
  agencyId?: string;
  studioId?: string;
  listingId?: string;
  status?: OrderStatus;
  keyword?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery<PagedResponse<ShootOrder>>({
    queryKey: ["orders", params],
    queryFn: () => ordersApi.list({
      ...params,
      status: params?.status?.toString(),
    }),
  });
}

export function useOrder(id: string | null) {
  return useQuery<ShootOrder>({
    queryKey: ["order", id],
    queryFn: () => ordersApi.getById(id!),
    enabled: !!id,
  });
}

export function useCreateOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ordersApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      toast.success("Order created successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to create order");
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
      toast.success("Order cancelled successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to cancel order");
    },
  });
}
