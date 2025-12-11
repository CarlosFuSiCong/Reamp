import { Badge } from "@/components/ui/badge";
import { ListingStatus, OrderStatus, DeliveryStatus } from "@/types";

interface StatusBadgeProps {
  status: ListingStatus | OrderStatus | DeliveryStatus;
  type?: "listing" | "order" | "delivery";
}

export function StatusBadge({ status, type = "listing" }: StatusBadgeProps) {
  const getStatusConfig = () => {
    if (type === "listing") {
      const listingStatus = status as ListingStatus;
      switch (listingStatus) {
        case ListingStatus.Draft:
          return { label: "Draft", variant: "secondary" as const };
        case ListingStatus.Active:
          return { label: "Active", variant: "default" as const };
        case ListingStatus.Pending:
          return { label: "Pending", variant: "outline" as const };
        case ListingStatus.Sold:
          return { label: "Sold", variant: "default" as const };
        case ListingStatus.Rented:
          return { label: "Rented", variant: "default" as const };
        case ListingStatus.Archived:
          return { label: "Archived", variant: "secondary" as const };
        default:
          return { label: "Unknown", variant: "secondary" as const };
      }
    } else if (type === "order") {
      const orderStatus = status as OrderStatus;
      switch (orderStatus) {
        case OrderStatus.Placed:
          return { label: "Placed", variant: "outline" as const };
        case OrderStatus.Accepted:
          return { label: "Accepted", variant: "secondary" as const };
        case OrderStatus.Scheduled:
          return { label: "Scheduled", variant: "default" as const };
        case OrderStatus.InProgress:
          return { label: "In Progress", variant: "default" as const };
        case OrderStatus.Completed:
          return { label: "Completed", variant: "default" as const };
        case OrderStatus.Cancelled:
          return { label: "Cancelled", variant: "destructive" as const };
        default:
          return { label: "Unknown", variant: "secondary" as const };
      }
    } else {
      const deliveryStatus = status as DeliveryStatus;
      switch (deliveryStatus) {
        case DeliveryStatus.Draft:
          return { label: "Draft", variant: "secondary" as const };
        case DeliveryStatus.Published:
          return { label: "Published", variant: "default" as const };
        case DeliveryStatus.Expired:
          return { label: "Expired", variant: "destructive" as const };
        case DeliveryStatus.Revoked:
          return { label: "Revoked", variant: "destructive" as const };
        default:
          return { label: "Unknown", variant: "secondary" as const };
      }
    }
  };

  const { label, variant } = getStatusConfig();

  return <Badge variant={variant}>{label}</Badge>;
}
