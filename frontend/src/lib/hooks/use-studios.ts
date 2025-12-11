import { useQuery } from "@tanstack/react-query";
import { studiosApi } from "@/lib/api";
import { Studio, PagedResponse } from "@/types";

export function useStudios(params?: {
  status?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery<PagedResponse<Studio>>({
    queryKey: ["studios", params],
    queryFn: async () => {
      try {
        return await studiosApi.list(params || {});
      } catch (error) {
        return { items: [], total: 0, page: 1, pageSize: 20 };
      }
    },
    retry: false,
    staleTime: 5 * 60 * 1000,
  });
}

export function useStudio(id: string | null) {
  return useQuery<Studio>({
    queryKey: ["studio", id],
    queryFn: () => studiosApi.getById(id!),
    enabled: !!id,
    retry: 1,
    staleTime: 5 * 60 * 1000,
  });
}
