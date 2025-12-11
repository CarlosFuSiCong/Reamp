import { AccessType, DeliveryStatus } from "./enums";

export interface DeliveryPackage {
  id: string;
  orderId: string;
  listingId: string;
  title: string;
  status: DeliveryStatus;
  watermarkEnabled: boolean;
  expiresAtUtc?: string;
  items: DeliveryItem[];
  accesses: DeliveryAccess[];
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface DeliveryItem {
  id: string;
  mediaAssetId: string;
  variantName: string;
  sortOrder: number;
}

export interface DeliveryAccess {
  id: string;
  type: AccessType;
  recipientEmail?: string;
  recipientName?: string;
  maxDownloads?: number;
  downloads: number;
  hasPassword: boolean;
}
