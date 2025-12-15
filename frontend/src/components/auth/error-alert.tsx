import { AlertCircle } from "lucide-react";
import { cn } from "@/lib/utils";

interface ErrorAlertProps {
  message?: string;
  className?: string;
}

export function ErrorAlert({ message, className }: ErrorAlertProps) {
  if (!message) return null;

  return (
    <div className={cn(
      "rounded-lg bg-red-50 dark:bg-red-900/10 p-4 border border-red-200 dark:border-red-900/30",
      "flex items-start gap-3",
      className
    )}>
      <AlertCircle className="h-5 w-5 text-red-600 dark:text-red-500 mt-0.5 shrink-0" />
      <p className="text-sm text-red-700 dark:text-red-400 flex-1">{message}</p>
    </div>
  );
}

