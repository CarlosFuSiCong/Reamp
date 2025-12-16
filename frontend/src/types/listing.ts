import { ListingStatus, ListingType, PropertyType, ListingMediaRole } from "./enums";

export interface Listing {
  id: string;
  ownerAgencyId: string;
  title: string;
  description: string;
  price: number;
  currency: string;
  status: ListingStatus;
  listingType: ListingType;
  propertyType: PropertyType;
  bedrooms: number;
  bathrooms: number;
  parkingSpaces: number;
  floorAreaSqm?: number;
  landAreaSqm?: number;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  postcode: string;
  country: string;
  media: ListingMedia[];
  agents: ListingAgent[];
}

export interface ListingMedia {
  id?: string;
  mediaAssetId: string;
  role: string | ListingMediaRole;
  sortOrder: number;
  isCover: boolean;
  isVisible?: boolean;
  thumbnailUrl?: string;
}

export interface ListingAgent {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  isPrimary: boolean;
  sortOrder: number;
}

// DTOs for media management
export interface AddMediaDto {
  mediaAssetId: string;
  role?: ListingMediaRole;
}

export interface ReorderMediaDto {
  mediaOrders: Record<string, number>;
}

export interface SetMediaVisibilityDto {
  mediaId: string;
  isVisible: boolean;
}
