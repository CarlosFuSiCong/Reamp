import { useQuery } from "@tanstack/react-query";
import { profilesApi } from "@/lib/api";

export function useUser() {
  const {
    data: user,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["user", "me"],
    queryFn: () => profilesApi.getMe(),
    staleTime: 5 * 60 * 1000,
    retry: false,
  });

  return {
    user,
    isLoading,
    error,
    refetch,
  };
}

export function useUserById(id: string) {
  const {
    data: user,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["user", id],
    queryFn: () => profilesApi.getById(id),
    enabled: !!id,
    staleTime: 5 * 60 * 1000,
  });

  return {
    user,
    isLoading,
    error,
  };
}
