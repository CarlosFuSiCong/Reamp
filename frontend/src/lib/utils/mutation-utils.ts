import { toast } from "sonner";

interface ErrorResponse {
  message?: string;
  errors?: string[];
}

export function handleMutationError(
  error: any,
  fallbackMessage: string,
  duration: number = 5000
): void {
  const errorResponse = error as ErrorResponse;
  const errorMessage = errorResponse?.message || fallbackMessage;
  
  toast.error(errorMessage, { duration });
  
  if (errorResponse?.errors && Array.isArray(errorResponse.errors)) {
    errorResponse.errors.forEach((err: string) => {
      toast.error(err, { duration });
    });
  }
}

export function handleMutationSuccess(
  message: string,
  description?: string
): void {
  toast.success(message, description ? { description } : undefined);
}
