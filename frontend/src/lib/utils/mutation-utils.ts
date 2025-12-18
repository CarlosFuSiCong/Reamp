import { showToast } from "./toast";

interface ErrorResponse {
  message?: string;
  errors?: string[];
}

export function handleMutationError(
  error: unknown,
  fallbackMessage: string,
  duration: number = 5000
): void {
  const errorResponse = error as ErrorResponse;
  const errorMessage = errorResponse?.message || fallbackMessage;

  showToast.error(errorMessage);

  if (errorResponse?.errors && Array.isArray(errorResponse.errors)) {
    errorResponse.errors.forEach((err: string) => {
      showToast.error(err);
    });
  }
}

export function handleMutationSuccess(message: string, description?: string): void {
  showToast.success(message, description);
}
