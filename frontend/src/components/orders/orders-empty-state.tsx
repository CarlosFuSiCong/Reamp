import { LucideIcon } from "lucide-react";
import { Card } from "@/components/ui/card";

interface OrdersEmptyStateProps {
  icon: LucideIcon;
  title: string;
  description: string;
  action?: React.ReactNode;
}

export function OrdersEmptyState({ 
  icon: Icon, 
  title, 
  description,
  action 
}: OrdersEmptyStateProps) {
  return (
    <Card className="p-12">
      <div className="text-center">
        <Icon className="mx-auto h-12 w-12 text-muted-foreground" />
        <h3 className="mt-4 text-lg font-semibold">{title}</h3>
        <p className="mt-2 text-sm text-muted-foreground">{description}</p>
        {action && <div className="mt-4">{action}</div>}
      </div>
    </Card>
  );
}
