import { useQuery } from "@tanstack/react-query";
import { studiosApi } from "@/lib/api/studios";

interface UseStudioStaffParams {
  studioId: string | null | undefined;
  search?: string;
  hasSkill?: number;
  page?: number;
  pageSize?: number;
}

/**
 * Custom hook to fetch staff members for a studio
 */
export function useStudioStaff({
  studioId,
  search,
  hasSkill,
  page = 1,
  pageSize = 50,
}: UseStudioStaffParams) {
  return useQuery({
    queryKey: ["studio-staff", studioId, search, hasSkill, page, pageSize],
    queryFn: () => {
      if (!studioId) throw new Error("Studio ID is required");
      return studiosApi.getStaffMembers({
        studioId,
        search,
        hasSkill,
        page,
        pageSize,
      });
    },
    enabled: !!studioId,
  });
}
