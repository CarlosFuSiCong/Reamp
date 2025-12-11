import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listingsApi } from "@/lib/api";

export function useListings(params?: {
  agencyId?: string;
  status?: string;
  type?: string;
  page?: number;
  pageSize?: number;
}) {
  const { data, isLoading, error, refetch } = useQuery({
    queryKey: ["listings", params],
    queryFn: () => listingsApi.list(params || {}),
    staleTime: 2 * 60 * 1000,
  });

  return {
    listings: data?.items || [],
    total: data?.total || 0,
    page: data?.page || 1,
    pageSize: data?.pageSize || 20,
    isLoading,
    error,
    refetch,
  };
}

export function useListing(id: string) {
  const {
    data: listing,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getById(id),
    enabled: !!id,
    staleTime: 2 * 60 * 1000,
  });

  return {
    listing,
    isLoading,
    error,
  };
}

export function useCreateListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: listingsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
    },
  });
}

export function useUpdateListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Parameters<typeof listingsApi.update>[1] }) =>
      listingsApi.update(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
      queryClient.invalidateQueries({ queryKey: ["listing", variables.id] });
    },
  });
}

export function useDeleteListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: listingsApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
    },
  });
}
