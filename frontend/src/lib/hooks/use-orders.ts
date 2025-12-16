import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersApi } from "@/lib/api";
import { ShootOrder, PagedResponse, OrderStatus } from "@/types";
import { handleMutationError, handleMutationSuccess } from "@/lib/utils";

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
    queryFn: () =>
      ordersApi.list({
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
      handleMutationSuccess("Order created successfully");
    },
    onError: (error: unknown) => {
      handleMutationError(error, "Failed to create order");
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
      handleMutationSuccess("Order cancelled successfully");
    },
    onError: (error: unknown) => {
      handleMutationError(error, "Failed to cancel order");
    },
  });
}

export function useStartOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => ordersApi.start(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      queryClient.invalidateQueries({ queryKey: ["photographer-orders"] });
      queryClient.invalidateQueries({ queryKey: ["order", id] });
      handleMutationSuccess("Order started successfully");
    },
    onError: (error: unknown) => {
      handleMutationError(error, "Failed to start order");
    },
  });
}

export function useCompleteOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => ordersApi.complete(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      queryClient.invalidateQueries({ queryKey: ["photographer-orders"] });
      queryClient.invalidateQueries({ queryKey: ["order", id] });
      handleMutationSuccess("Order completed successfully");
    },
    onError: (error: unknown) => {
      handleMutationError(error, "Failed to complete order");
    },
  });
}

// Photographer-specific hooks
export function useAvailableOrders(params?: { page?: number; pageSize?: number }) {
  return useQuery<PagedResponse<ShootOrder>>({
    queryKey: ["available-orders", params],
    queryFn: () => ordersApi.getAvailableOrders(params || {}),
  });
}

export function usePhotographerOrders(params?: {
  status?: OrderStatus;
  page?: number;
  pageSize?: number;
}) {
  return useQuery<PagedResponse<ShootOrder>>({
    queryKey: ["photographer-orders", params],
    queryFn: () =>
      ordersApi.getMyOrders({
        ...params,
        status: params?.status?.toString(),
      }),
  });
}

export function useAcceptOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => ordersApi.acceptAsPhotographer(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ["available-orders"] });
      queryClient.invalidateQueries({ queryKey: ["photographer-orders"] });
      queryClient.invalidateQueries({ queryKey: ["order", id] });
      handleMutationSuccess("Order accepted successfully");
    },
    onError: (error: unknown) => {
      handleMutationError(error, "Failed to accept order");
    },
  });
}
