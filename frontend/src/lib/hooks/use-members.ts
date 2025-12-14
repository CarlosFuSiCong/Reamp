import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { membersApi } from "@/lib/api";
import {
  AgencyMemberDto,
  StudioMemberDto,
  UpdateAgencyMemberRoleDto,
  UpdateStudioMemberRoleDto,
} from "@/types";
import { toast } from "sonner";

// Get agency members
export function useAgencyMembers(agencyId: string) {
  return useQuery<AgencyMemberDto[]>({
    queryKey: ["agencies", agencyId, "members"],
    queryFn: () => membersApi.getAgencyMembers(agencyId),
    enabled: !!agencyId,
  });
}

// Update agency member role
export function useUpdateAgencyMemberRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      agencyId,
      memberId,
      data,
    }: {
      agencyId: string;
      memberId: string;
      data: UpdateAgencyMemberRoleDto;
    }) => membersApi.updateAgencyMemberRole(agencyId, memberId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["agencies", variables.agencyId, "members"],
      });
      toast.success("Member role updated", {
        description: "The member's role has been updated successfully",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to update member role", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Remove agency member
export function useRemoveAgencyMember() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      agencyId,
      memberId,
    }: {
      agencyId: string;
      memberId: string;
    }) => membersApi.removeAgencyMember(agencyId, memberId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["agencies", variables.agencyId, "members"],
      });
      toast.success("Member removed", {
        description: "The member has been removed from the agency",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to remove member", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Get studio members
export function useStudioMembers(studioId: string) {
  return useQuery<StudioMemberDto[]>({
    queryKey: ["studios", studioId, "members"],
    queryFn: () => membersApi.getStudioMembers(studioId),
    enabled: !!studioId,
  });
}

// Update studio member role
export function useUpdateStudioMemberRole() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      studioId,
      memberId,
      data,
    }: {
      studioId: string;
      memberId: string;
      data: UpdateStudioMemberRoleDto;
    }) => membersApi.updateStudioMemberRole(studioId, memberId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["studios", variables.studioId, "members"],
      });
      toast.success("Member role updated", {
        description: "The member's role has been updated successfully",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to update member role", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Remove studio member
export function useRemoveStudioMember() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      studioId,
      memberId,
    }: {
      studioId: string;
      memberId: string;
    }) => membersApi.removeStudioMember(studioId, memberId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["studios", variables.studioId, "members"],
      });
      toast.success("Member removed", {
        description: "The member has been removed from the studio",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to remove member", {
        description: error?.message || "Please try again",
      });
    },
  });
}
