import { Badge } from "@/components/ui/badge";
import { ListingStatus, OrderStatus, DeliveryStatus } from "@/types";
import {
  getListingStatusConfig,
  getOrderStatusConfig,
  getDeliveryStatusConfig,
} from "@/lib/utils/enum-labels";

interface StatusBadgeProps {
  status: ListingStatus | OrderStatus | DeliveryStatus;
  type?: "listing" | "order" | "delivery";
}

export function StatusBadge({ status, type = "listing" }: StatusBadgeProps) {
  const getConfig = () => {
    switch (type) {
      case "listing":
        return getListingStatusConfig(status as ListingStatus);
      case "order":
        return getOrderStatusConfig(status as OrderStatus);
      case "delivery":
        return getDeliveryStatusConfig(status as DeliveryStatus);
      default:
        return { label: "Unknown", variant: "secondary" as const };
    }
  };

  const { label, variant } = getConfig();

  return <Badge variant={variant}>{label}</Badge>;
}
