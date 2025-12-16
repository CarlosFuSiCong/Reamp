import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { invitationsApi } from "@/lib/api";
import {
  InvitationListDto,
  InvitationDetailDto,
  SendAgencyInvitationDto,
  SendStudioInvitationDto,
} from "@/types";
import { handleMutationError, handleMutationSuccess } from "@/lib/utils";

export function useMyInvitations() {
  return useQuery<InvitationListDto[]>({
    queryKey: ["invitations", "me"],
    queryFn: () => invitationsApi.getMyInvitations(),
  });
}

export function useInvitation(invitationId: string) {
  return useQuery<InvitationDetailDto>({
    queryKey: ["invitations", invitationId],
    queryFn: () => invitationsApi.getInvitationById(invitationId),
    enabled: !!invitationId,
  });
}

export function useAcceptInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (invitationId: string) => invitationsApi.acceptInvitation(invitationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      handleMutationSuccess("Invitation accepted", "You have successfully joined the team!");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to accept invitation");
    },
  });
}

export function useRejectInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (invitationId: string) => invitationsApi.rejectInvitation(invitationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      handleMutationSuccess("Invitation rejected", "The invitation has been declined");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to reject invitation");
    },
  });
}

export function useCancelInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (invitationId: string) => invitationsApi.cancelInvitation(invitationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      handleMutationSuccess("Invitation cancelled", "The invitation has been cancelled");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to cancel invitation");
    },
  });
}

export function useSendAgencyInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ agencyId, data }: { agencyId: string; data: SendAgencyInvitationDto }) =>
      invitationsApi.sendAgencyInvitation(agencyId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      handleMutationSuccess("Invitation sent", "The invitation has been sent successfully");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to send invitation");
    },
  });
}

export function useAgencyInvitations(agencyId: string) {
  return useQuery<InvitationDetailDto[]>({
    queryKey: ["agencies", agencyId, "invitations"],
    queryFn: () => invitationsApi.getAgencyInvitations(agencyId),
    enabled: !!agencyId,
  });
}

export function useSendStudioInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ studioId, data }: { studioId: string; data: SendStudioInvitationDto }) =>
      invitationsApi.sendStudioInvitation(studioId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      handleMutationSuccess("Invitation sent", "The invitation has been sent successfully");
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to send invitation");
    },
  });
}

export function useStudioInvitations(studioId: string) {
  return useQuery<InvitationDetailDto[]>({
    queryKey: ["studios", studioId, "invitations"],
    queryFn: () => invitationsApi.getStudioInvitations(studioId),
    enabled: !!studioId,
  });
}
