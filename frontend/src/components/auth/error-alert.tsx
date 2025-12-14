import { cn } from "@/lib/utils";

interface ErrorAlertProps {
  message?: string;
  className?: string;
}

export function ErrorAlert({ message, className }: ErrorAlertProps) {
  if (!message) return null;

  return (
    <div className={cn("rounded-md bg-destructive/10 p-4 border border-destructive/20", className)}>
      <p className="text-sm text-destructive">{message}</p>
    </div>
  );
}

