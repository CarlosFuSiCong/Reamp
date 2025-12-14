import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { membersApi } from "@/lib/api";
import {
  AgencyMemberDto,
  StudioMemberDto,
  UpdateAgencyMemberRoleDto,
  UpdateStudioMemberRoleDto,
} from "@/types";
import { handleMutationError, handleMutationSuccess } from "@/lib/utils";

export function useAgencyMembers(agencyId: string) {
  return useQuery<AgencyMemberDto[]>({
    queryKey: ["agencies", agencyId, "members"],
    queryFn: () => membersApi.getAgencyMembers(agencyId),
    enabled: !!agencyId,
  });
}

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
      handleMutationSuccess("Member role updated", "The member's role has been updated successfully");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to update member role");
    },
  });
}

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
      handleMutationSuccess("Member removed", "The member has been removed from the agency");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to remove member");
    },
  });
}

export function useStudioMembers(studioId: string) {
  return useQuery<StudioMemberDto[]>({
    queryKey: ["studios", studioId, "members"],
    queryFn: () => membersApi.getStudioMembers(studioId),
    enabled: !!studioId,
  });
}

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
      handleMutationSuccess("Member role updated", "The member's role has been updated successfully");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to update member role");
    },
  });
}

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
      handleMutationSuccess("Member removed", "The member has been removed from the studio");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to remove member");
    },
  });
}
