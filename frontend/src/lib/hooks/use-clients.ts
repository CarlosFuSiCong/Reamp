import { useQuery } from "@tanstack/react-query";
import { clientsApi } from "@/lib/api";
import { Client, PagedResponse } from "@/types";

export function useClients(params?: {
  agencyId?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery<PagedResponse<Client>>({
    queryKey: ["clients", params],
    queryFn: () => clientsApi.list(params || {}),
    retry: 1,
    staleTime: 5 * 60 * 1000,
  });
}

export function useClient(id: string | null) {
  return useQuery<Client>({
    queryKey: ["client", id],
    queryFn: () => clientsApi.getById(id!),
    enabled: !!id,
    retry: 1,
    staleTime: 5 * 60 * 1000,
  });
}
