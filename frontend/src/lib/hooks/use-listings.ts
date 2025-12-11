import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listingsApi } from "@/lib/api/listings";
import { Listing, PagedResponse, ListingStatus } from "@/types";
import { toast } from "sonner";

export function useListings(params?: {
  status?: ListingStatus;
  keyword?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery<PagedResponse<Listing>>({
    queryKey: ["listings", params],
    queryFn: () => listingsApi.list(params || {}),
  });
}

export function useListing(id: string | null) {
  return useQuery<Listing>({
    queryKey: ["listing", id],
    queryFn: () => listingsApi.getById(id!),
    enabled: !!id,
  });
}

export function useCreateListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: Partial<Listing>) => listingsApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
      toast.success("Listing created successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to create listing");
    },
  });
}

export function useUpdateListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<Listing> }) =>
      listingsApi.update(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
      queryClient.invalidateQueries({ queryKey: ["listing", variables.id] });
      toast.success("Listing updated successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to update listing");
    },
  });
}

export function useDeleteListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => listingsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
      toast.success("Listing deleted successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to delete listing");
    },
  });
}

export function usePublishListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => listingsApi.publish(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
      queryClient.invalidateQueries({ queryKey: ["listing", id] });
      toast.success("Listing published successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to publish listing");
    },
  });
}

export function useArchiveListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => listingsApi.archive(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ["listings"] });
      queryClient.invalidateQueries({ queryKey: ["listing", id] });
      toast.success("Listing archived successfully");
    },
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || "Failed to archive listing");
    },
  });
}
