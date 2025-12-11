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
    queryFn: () => studiosApi.list(params || {}),
  });
}

export function useStudio(id: string | null) {
  return useQuery<Studio>({
    queryKey: ["studio", id],
    queryFn: () => studiosApi.getById(id!),
    enabled: !!id,
  });
}
