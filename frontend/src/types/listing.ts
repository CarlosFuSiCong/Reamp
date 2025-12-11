import { ListingStatus, ListingType, PropertyType } from "./enums";

export interface Listing {
  id: string;
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
  mediaAssetId: string;
  role: string;
  sortOrder: number;
  isCover: boolean;
}

export interface ListingAgent {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  isPrimary: boolean;
  sortOrder: number;
}
