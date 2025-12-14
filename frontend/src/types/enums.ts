export enum UserRole {
  None = 0,
  Client = 1,
  Agent = 2,
  Staff = 3,
  Admin = 4,
}

export enum AgencyRole {
  Agent = 1,
  Manager = 2,
  Owner = 3,
}

export enum StudioRole {
  Staff = 1,
  Manager = 2,
  Owner = 3,
}

export enum UserStatus {
  Active = 0,
  Inactive = 1,
}

export enum AccountStatus {
  Active = 0,
  Inactive = 1,
  Suspended = 2,
}

export enum StaffRole {
  Photographer = 1,
  Videographer = 2,
  Editor = 3,
  Manager = 4,
}

export enum ListingType {
  ForSale = 1,
  ForRent = 2,
  Auction = 3,
}

export enum PropertyType {
  House = 1,
  Townhouse = 2,
  Villa = 3,
  Apartment = 4,
  Others = 5,
}

export enum ListingStatus {
  Draft = 0,
  Active = 1,
  Pending = 2,
  Sold = 3,
  Rented = 4,
  Archived = 5,
}

export enum OrderStatus {
  Placed = 1,
  Accepted = 2,
  Scheduled = 3,
  InProgress = 4,
  Completed = 5,
  Cancelled = 6,
}

export enum TaskStatus {
  Pending = 1,
  Scheduled = 2,
  InProgress = 3,
  Done = 4,
  Cancelled = 5,
}

export enum DeliveryStatus {
  Draft = 1,
  Published = 2,
  Expired = 3,
  Revoked = 4,
}

export enum AccessType {
  Public = 1,
  Token = 2,
  Private = 3,
}

export enum MediaProcessStatus {
  Unknown = 0,
  Uploaded = 1,
  Processing = 2,
  Ready = 3,
  Failed = 4,
}
