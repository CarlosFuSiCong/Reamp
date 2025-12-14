import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { invitationsApi } from "@/lib/api";
import {
  InvitationListDto,
  InvitationDetailDto,
  SendAgencyInvitationDto,
  SendStudioInvitationDto,
} from "@/types";
import { toast } from "sonner";

// Get my invitations
export function useMyInvitations() {
  return useQuery<InvitationListDto[]>({
    queryKey: ["invitations", "me"],
    queryFn: () => invitationsApi.getMyInvitations(),
  });
}

// Get invitation by ID
export function useInvitation(invitationId: string) {
  return useQuery<InvitationDetailDto>({
    queryKey: ["invitations", invitationId],
    queryFn: () => invitationsApi.getInvitationById(invitationId),
    enabled: !!invitationId,
  });
}

// Accept invitation
export function useAcceptInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (invitationId: string) =>
      invitationsApi.acceptInvitation(invitationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      toast.success("Invitation accepted", {
        description: "You have successfully joined the team!",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to accept invitation", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Reject invitation
export function useRejectInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (invitationId: string) =>
      invitationsApi.rejectInvitation(invitationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      toast.success("Invitation rejected", {
        description: "The invitation has been declined",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to reject invitation", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Cancel invitation
export function useCancelInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (invitationId: string) =>
      invitationsApi.cancelInvitation(invitationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      toast.success("Invitation cancelled", {
        description: "The invitation has been cancelled",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to cancel invitation", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Send agency invitation
export function useSendAgencyInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      agencyId,
      data,
    }: {
      agencyId: string;
      data: SendAgencyInvitationDto;
    }) => invitationsApi.sendAgencyInvitation(agencyId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      toast.success("Invitation sent", {
        description: "The invitation has been sent successfully",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to send invitation", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Get agency invitations
export function useAgencyInvitations(agencyId: string) {
  return useQuery<InvitationDetailDto[]>({
    queryKey: ["agencies", agencyId, "invitations"],
    queryFn: () => invitationsApi.getAgencyInvitations(agencyId),
    enabled: !!agencyId,
  });
}

// Send studio invitation
export function useSendStudioInvitation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      studioId,
      data,
    }: {
      studioId: string;
      data: SendStudioInvitationDto;
    }) => invitationsApi.sendStudioInvitation(studioId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["invitations"] });
      toast.success("Invitation sent", {
        description: "The invitation has been sent successfully",
      });
    },
    onError: (error: any) => {
      toast.error("Failed to send invitation", {
        description: error?.message || "Please try again",
      });
    },
  });
}

// Get studio invitations
export function useStudioInvitations(studioId: string) {
  return useQuery<InvitationDetailDto[]>({
    queryKey: ["studios", studioId, "invitations"],
    queryFn: () => invitationsApi.getStudioInvitations(studioId),
    enabled: !!studioId,
  });
}
