import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { staffApi, StaffDetailDto } from "@/lib/api/staff";
import { StaffSkills } from "@/types/enums";
import { toast } from "sonner";

export function useStaff(staffId: string | null) {
  return useQuery<StaffDetailDto | null>({
    queryKey: ["staff", staffId],
    queryFn: () => (staffId ? staffApi.getById(staffId) : null),
    enabled: !!staffId,
  });
}

export function useStaffByUserProfileId(userProfileId: string | null) {
  return useQuery<StaffDetailDto | null>({
    queryKey: ["staff", "profile", userProfileId],
    queryFn: () => (userProfileId ? staffApi.getByUserProfileId(userProfileId) : null),
    enabled: !!userProfileId,
  });
}

export function useUpdateStaffSkills() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ staffId, skills }: { staffId: string; skills: StaffSkills }) =>
      staffApi.updateSkills(staffId, skills),
    onSuccess: (data) => {
      // Invalidate relevant queries
      queryClient.invalidateQueries({ queryKey: ["staff", data.id] });
      queryClient.invalidateQueries({ queryKey: ["staff", "profile", data.userProfileId] });

      toast.success("Skills updated successfully");
    },
    onError: (error: unknown) => {
      toast.error(error.message || "Failed to update skills");
    },
  });
}
