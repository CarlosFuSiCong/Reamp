import { Badge } from "@/components/ui/badge";
import { DeliveryStatus } from "@/types";
import { getDeliveryStatusConfig } from "@/lib/utils/enum-labels";

interface DeliveryStatusBadgeProps {
  status: DeliveryStatus;
  className?: string;
}

export function DeliveryStatusBadge({ status, className }: DeliveryStatusBadgeProps) {
  const config = getDeliveryStatusConfig(status);

  return (
    <Badge variant={config.variant} className={className}>
      {config.label}
    </Badge>
  );
}
