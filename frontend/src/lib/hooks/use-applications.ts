import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { applicationsApi } from "@/lib/api";
import { ApplicationStatus, ApplicationType } from "@/types";
import { handleMutationError, handleMutationSuccess } from "@/lib/utils";

export function useApplications(
  page: number = 1,
  pageSize: number = 20,
  status?: ApplicationStatus,
  type?: ApplicationType
) {
  return useQuery({
    queryKey: ["applications", page, pageSize, status, type],
    queryFn: () => applicationsApi.getApplications(page, pageSize, status, type),
  });
}

export function useMyApplications() {
  return useQuery({
    queryKey: ["applications", "my"],
    queryFn: () => applicationsApi.getMyApplications(),
  });
}

export function useApplicationDetail(id: string) {
  return useQuery({
    queryKey: ["applications", id],
    queryFn: () => applicationsApi.getApplicationDetail(id),
    enabled: !!id,
  });
}

export function useSubmitAgencyApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: applicationsApi.submitAgencyApplication,
    onSuccess: () => {
      handleMutationSuccess("Agency application submitted successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to submit agency application");
    },
  });
}

export function useSubmitStudioApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: applicationsApi.submitStudioApplication,
    onSuccess: () => {
      handleMutationSuccess("Studio application submitted successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to submit studio application");
    },
  });
}

export function useReviewApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, approved, notes }: { id: string; approved: boolean; notes?: string }) =>
      applicationsApi.reviewApplication(id, { approved, notes }),
    onSuccess: () => {
      handleMutationSuccess("Application reviewed successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to review application");
    },
  });
}

export function useCancelApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => applicationsApi.cancelApplication(id),
    onSuccess: () => {
      handleMutationSuccess("Application cancelled successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      handleMutationError(error, "Failed to cancel application");
    },
  });
}
