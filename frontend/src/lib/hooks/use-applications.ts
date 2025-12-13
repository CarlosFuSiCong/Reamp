import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { applicationsApi } from "@/lib/api";
import { ApplicationStatus, ApplicationType } from "@/types";
import { toast } from "sonner";

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
      toast.success("Agency application submitted successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      const errorMessage = error?.message || "Failed to submit agency application";
      const errors = error?.errors;
      
      toast.error(errorMessage, { duration: 5000 });
      
      if (errors && Array.isArray(errors) && errors.length > 0) {
        errors.forEach((err: string) => {
          toast.error(err, { duration: 5000 });
        });
      }
    },
  });
}

export function useSubmitStudioApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: applicationsApi.submitStudioApplication,
    onSuccess: () => {
      toast.success("Studio application submitted successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      const errorMessage = error?.message || "Failed to submit studio application";
      const errors = error?.errors;
      
      toast.error(errorMessage, { duration: 5000 });
      
      if (errors && Array.isArray(errors) && errors.length > 0) {
        errors.forEach((err: string) => {
          toast.error(err, { duration: 5000 });
        });
      }
    },
  });
}

export function useReviewApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, approved, notes }: { id: string; approved: boolean; notes?: string }) =>
      applicationsApi.reviewApplication(id, { approved, notes }),
    onSuccess: () => {
      toast.success("Application reviewed successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      const errorMessage = error?.message || "Failed to review application";
      const errors = error?.errors;
      
      // Show main error message with longer duration
      toast.error(errorMessage, { duration: 5000 });
      
      // Show additional error details if available
      if (errors && Array.isArray(errors) && errors.length > 0) {
        errors.forEach((err: string) => {
          toast.error(err, { duration: 5000 });
        });
      }
    },
  });
}

export function useCancelApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => applicationsApi.cancelApplication(id),
    onSuccess: () => {
      toast.success("Application cancelled successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      const errorMessage = error?.message || "Failed to cancel application";
      const errors = error?.errors;
      
      toast.error(errorMessage, { duration: 5000 });
      
      if (errors && Array.isArray(errors) && errors.length > 0) {
        errors.forEach((err: string) => {
          toast.error(err, { duration: 5000 });
        });
      }
    },
  });
}
