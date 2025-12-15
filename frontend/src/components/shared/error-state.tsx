import { Card, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";

interface ErrorStateProps {
  title?: string;
  message?: string;
}

export function ErrorState({ 
  title = "Error", 
  message = "Something went wrong" 
}: ErrorStateProps) {
  return (
    <div className="flex items-center justify-center min-h-screen">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>{title}</CardTitle>
          <CardDescription>{message}</CardDescription>
        </CardHeader>
      </Card>
    </div>
  );
}








