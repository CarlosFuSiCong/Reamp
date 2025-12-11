import { ListingStatus, ListingType, PropertyType, OrderStatus, DeliveryStatus, ShootTaskType } from "@/types";

type BadgeVariant = "default" | "secondary" | "outline" | "destructive";

interface StatusConfig {
  label: string;
  variant: BadgeVariant;
}

export const listingStatusConfig: Record<ListingStatus, StatusConfig> = {
  [ListingStatus.Draft]: { label: "Draft", variant: "secondary" },
  [ListingStatus.Active]: { label: "Active", variant: "default" },
  [ListingStatus.Pending]: { label: "Pending", variant: "outline" },
  [ListingStatus.Sold]: { label: "Sold", variant: "default" },
  [ListingStatus.Rented]: { label: "Rented", variant: "default" },
  [ListingStatus.Archived]: { label: "Archived", variant: "secondary" },
};

export const orderStatusConfig: Record<OrderStatus, StatusConfig> = {
  [OrderStatus.Placed]: { label: "Placed", variant: "outline" },
  [OrderStatus.Accepted]: { label: "Accepted", variant: "secondary" },
  [OrderStatus.Scheduled]: { label: "Scheduled", variant: "default" },
  [OrderStatus.InProgress]: { label: "In Progress", variant: "default" },
  [OrderStatus.Completed]: { label: "Completed", variant: "default" },
  [OrderStatus.Cancelled]: { label: "Cancelled", variant: "destructive" },
};

export const deliveryStatusConfig: Record<DeliveryStatus, StatusConfig> = {
  [DeliveryStatus.Draft]: { label: "Draft", variant: "secondary" },
  [DeliveryStatus.Published]: { label: "Published", variant: "default" },
  [DeliveryStatus.Expired]: { label: "Expired", variant: "destructive" },
  [DeliveryStatus.Revoked]: { label: "Revoked", variant: "destructive" },
};

export const listingTypeLabels: Record<ListingType, string> = {
  [ListingType.ForSale]: "For Sale",
  [ListingType.ForRent]: "For Rent",
  [ListingType.Auction]: "Auction",
};

export const propertyTypeLabels: Record<PropertyType, string> = {
  [PropertyType.House]: "House",
  [PropertyType.Apartment]: "Apartment",
  [PropertyType.Townhouse]: "Townhouse",
  [PropertyType.Villa]: "Villa",
  [PropertyType.Others]: "Others",
};

export const taskTypeLabels: Record<ShootTaskType, string> = {
  [ShootTaskType.None]: "None",
  [ShootTaskType.Photography]: "Photography",
  [ShootTaskType.Video]: "Video",
  [ShootTaskType.Floorplan]: "Floorplan",
};

export function getListingStatusConfig(status: ListingStatus): StatusConfig {
  return listingStatusConfig[status] || { label: "Unknown", variant: "secondary" };
}

export function getOrderStatusConfig(status: OrderStatus): StatusConfig {
  return orderStatusConfig[status] || { label: "Unknown", variant: "secondary" };
}

export function getDeliveryStatusConfig(status: DeliveryStatus): StatusConfig {
  return deliveryStatusConfig[status] || { label: "Unknown", variant: "secondary" };
}

export function getListingTypeLabel(type: ListingType): string {
  return listingTypeLabels[type] || "Unknown";
}

export function getPropertyTypeLabel(type: PropertyType): string {
  return propertyTypeLabels[type] || "Unknown";
}

export function getTaskTypeLabel(type: ShootTaskType): string {
  return taskTypeLabels[type] || "Unknown";
}
