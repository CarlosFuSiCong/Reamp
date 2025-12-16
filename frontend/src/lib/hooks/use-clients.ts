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
    queryFn: async () => {
      try {
        return await clientsApi.list(params || {});
      } catch {
        return { items: [], total: 0, page: 1, pageSize: 20 };
      }
    },
    retry: false,
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
