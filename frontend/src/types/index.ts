// API Response Types
export interface ApiResponse<T = any> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}

// User and Auth Types
export interface User {
  id: string;
  email: string;
  roles: string[];
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

export interface UserProfile {
  id: string;
  applicationUserId: string;
  displayName: string;
  avatarUrl?: string;
  phoneNumber?: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}

// Enums
export enum UserRole {
  Admin = "Admin",
  Agent = "Agent",
  Photographer = "Photographer",
  Client = "Client",
}

export enum AccountStatus {
  Active = "Active",
  Inactive = "Inactive",
  Suspended = "Suspended",
}

export enum StaffRole {
  Owner = "Owner",
  Manager = "Manager",
  Photographer = "Photographer",
  Editor = "Editor",
}

export enum ListingType {
  ForSale = "ForSale",
  ForRent = "ForRent",
  ForLease = "ForLease",
}

export enum PropertyType {
  House = "House",
  Apartment = "Apartment",
  Townhouse = "Townhouse",
  Villa = "Villa",
  Land = "Land",
  Commercial = "Commercial",
  Industrial = "Industrial",
  Rural = "Rural",
}

export enum ListingStatus {
  Draft = "Draft",
  Active = "Active",
  Archived = "Archived",
}

export enum OrderStatus {
  Pending = "Pending",
  Accepted = "Accepted",
  Scheduled = "Scheduled",
  InProgress = "InProgress",
  Completed = "Completed",
  Cancelled = "Cancelled",
}

export enum TaskStatus {
  Pending = "Pending",
  InProgress = "InProgress",
  Completed = "Completed",
}

export enum DeliveryStatus {
  Draft = "Draft",
  Published = "Published",
  Revoked = "Revoked",
  Expired = "Expired",
}

export enum AccessType {
  Public = "Public",
  Password = "Password",
  Email = "Email",
}

// Address Value Object
export interface Address {
  street: string;
  city: string;
  state: string;
  postcode: string;
  country: string;
}

// Agency Types
export interface Agency {
  id: string;
  name: string;
  slug: string;
  logoUrl?: string;
  contactEmail?: string;
  contactPhone?: string;
  status: AccountStatus;
  createdAtUtc: string;
}

export interface AgencyBranch {
  id: string;
  agencyId: string;
  name: string;
  slug: string;
  address?: Address;
  contactEmail?: string;
  contactPhone?: string;
  status: AccountStatus;
  createdAtUtc: string;
}

// Studio Types
export interface Studio {
  id: string;
  name: string;
  slug: string;
  logoUrl?: string;
  address?: Address;
  contactEmail?: string;
  contactPhone?: string;
  status: AccountStatus;
  createdAtUtc: string;
}

// Client Types
export interface Client {
  id: string;
  userProfileId: string;
  agencyId: string;
  branchId?: string;
  status: AccountStatus;
  createdAtUtc: string;
}

// Staff Types
export interface Staff {
  id: string;
  userProfileId: string;
  studioId: string;
  role: StaffRole;
  status: AccountStatus;
  createdAtUtc: string;
}

// Listing Types
export interface ListingMedia {
  id: string;
  mediaAssetId: string;
  url: string;
  type: string;
  sortOrder: number;
  isVisible: boolean;
}

export interface ListingAgent {
  id: string;
  clientId: string;
  displayName: string;
  avatarUrl?: string;
  isPrimary: boolean;
  sortOrder: number;
}

export interface Listing {
  id: string;
  title: string;
  description?: string;
  price: number;
  currency: string;
  listingType: ListingType;
  propertyType: PropertyType;
  status: ListingStatus;
  address: Address;
  bedroomCount?: number;
  bathroomCount?: number;
  parkingCount?: number;
  landArea?: number;
  buildingArea?: number;
  media: ListingMedia[];
  agents: ListingAgent[];
  createdAtUtc: string;
  updatedAtUtc: string;
  publishedAtUtc?: string;
  deletedAtUtc?: string;
}

export interface ListingListDto {
  id: string;
  title: string;
  price: number;
  currency: string;
  listingType: ListingType;
  propertyType: PropertyType;
  status: ListingStatus;
  address: Address;
  featuredImageUrl?: string;
  bedroomCount?: number;
  bathroomCount?: number;
  createdAtUtc: string;
}

// Order Types
export interface ShootTask {
  id: string;
  taskType: string;
  description: string;
  unitPrice: number;
  status: TaskStatus;
}

export interface ShootOrder {
  id: string;
  listingId: string;
  clientId: string;
  studioId: string;
  assignedPhotographerId?: string;
  status: OrderStatus;
  currency: string;
  totalAmount: number;
  tasks: ShootTask[];
  scheduledStartUtc?: string;
  scheduledEndUtc?: string;
  actualStartUtc?: string;
  actualEndUtc?: string;
  cancellationReason?: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface OrderListDto {
  id: string;
  listingTitle: string;
  status: OrderStatus;
  totalAmount: number;
  currency: string;
  assignedPhotographerName?: string;
  scheduledStartUtc?: string;
  createdAtUtc: string;
}

// Delivery Types
export interface DeliveryItem {
  id: string;
  mediaAssetId: string;
  url: string;
  fileName: string;
  fileSize: number;
}

export interface DeliveryAccess {
  id: string;
  type: AccessType;
  recipientEmail?: string;
  recipientName?: string;
  maxDownloads?: number;
  downloadCount: number;
  lastAccessedUtc?: string;
}

export interface DeliveryPackage {
  id: string;
  orderId?: string;
  listingId?: string;
  title: string;
  status: DeliveryStatus;
  watermarkEnabled: boolean;
  expiresAtUtc?: string;
  publishedAtUtc?: string;
  revokedAtUtc?: string;
  items: DeliveryItem[];
  accesses: DeliveryAccess[];
  createdAtUtc: string;
}

// Form DTOs
export interface LoginDto {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterDto {
  email: string;
  password: string;
  confirmPassword: string;
  displayName: string;
  phoneNumber?: string;
  role: UserRole;
}

export interface CreateListingDto {
  title: string;
  description?: string;
  price: number;
  currency: string;
  listingType: ListingType;
  propertyType: PropertyType;
  address: Address;
  bedroomCount?: number;
  bathroomCount?: number;
  parkingCount?: number;
  landArea?: number;
  buildingArea?: number;
}

export interface UpdateListingDto {
  title?: string;
  description?: string;
  price?: number;
  bedroomCount?: number;
  bathroomCount?: number;
  parkingCount?: number;
  landArea?: number;
  buildingArea?: number;
}

export interface PlaceOrderDto {
  listingId: string;
  clientId: string;
  studioId: string;
  currency: string;
  tasks: Array<{
    taskType: string;
    description: string;
    unitPrice: number;
  }>;
}

export interface CreateDeliveryPackageDto {
  orderId?: string;
  listingId?: string;
  title: string;
  watermarkEnabled: boolean;
  expiresAtUtc?: string;
}
