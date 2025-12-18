import { AlertCircle } from "lucide-react";
import { Card, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";

interface ErrorStateProps {
  title?: string;
  message?: string;
}

export function ErrorState({ 
  title = "Unable to Load Content", 
  message = "We encountered an error while loading this page. Please try again or contact support if the problem persists." 
}: ErrorStateProps) {
  return (
    <div className="flex items-center justify-center min-h-screen p-4">
      <Card className="w-full max-w-md border-red-200 bg-red-50/50">
        <CardHeader>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-full bg-red-100 flex items-center justify-center flex-shrink-0">
              <AlertCircle className="h-5 w-5 text-red-600" aria-hidden="true" />
            </div>
            <div className="flex-1">
              <CardTitle className="text-red-900">{title}</CardTitle>
              <CardDescription className="text-red-700 mt-1.5">{message}</CardDescription>
            </div>
          </div>
        </CardHeader>
      </Card>
    </div>
  );
}
