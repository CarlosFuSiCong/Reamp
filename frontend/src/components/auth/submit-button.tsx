import { Button } from "@/components/ui/button";

interface SubmitButtonProps {
  isLoading?: boolean;
  loadingText?: string;
  children: React.ReactNode;
}

export function SubmitButton({ isLoading, loadingText, children }: SubmitButtonProps) {
  return (
    <Button type="submit" disabled={isLoading} className="w-full">
      {isLoading ? loadingText : children}
    </Button>
  );
}
