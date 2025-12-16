import { AccessType, DeliveryStatus } from "./enums";

// Delivery Package DTOs
export interface DeliveryPackageDetailDto {
  id: string;
  orderId: string;
  listingId: string;
  title: string;
  status: DeliveryStatus;
  watermarkEnabled: boolean;
  expiresAtUtc?: string;
  items: DeliveryItemDto[];
  accesses: DeliveryAccessDto[];
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface DeliveryPackageListDto {
  id: string;
  orderId: string;
  listingId: string;
  title: string;
  status: DeliveryStatus;
  itemCount: number;
  createdAtUtc: string;
}

export interface DeliveryItemDto {
  id: string;
  mediaAssetId: string;
  variantName: string;
  sortOrder: number;
  mediaUrl?: string;
  thumbnailUrl?: string;
}

export interface DeliveryAccessDto {
  id: string;
  type: AccessType;
  recipientEmail?: string;
  recipientName?: string;
  maxDownloads?: number;
  downloads: number;
  hasPassword: boolean;
}

// Create/Update DTOs
export interface CreateDeliveryPackageDto {
  orderId: string;
  listingId: string;
  title: string;
  watermarkEnabled?: boolean;
  expiresAtUtc?: string;
}

export interface UpdateDeliveryPackageDto {
  title?: string;
  watermarkEnabled?: boolean;
  expiresAtUtc?: string;
}

export interface AddDeliveryItemDto {
  mediaAssetId: string;
  variantName?: string;
  sortOrder?: number;
}

export interface AddDeliveryAccessDto {
  type: AccessType;
  recipientEmail?: string;
  recipientName?: string;
  maxDownloads?: number;
  password?: string;
}

// Legacy type for backward compatibility
export type DeliveryPackage = DeliveryPackageDetailDto;
export type DeliveryItem = DeliveryItemDto;
export type DeliveryAccess = DeliveryAccessDto;
