export enum UserRole {
  None = 0,
  User = 1,
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

// Staff Skills (Flags enum)
export enum StaffSkills {
  None = 0,
  Photographer = 1, // 1 << 0
  Videographer = 2, // 1 << 1
  VRMaker = 4, // 1 << 2
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
  Placed = 1,              // Order placed, waiting for Studio to accept
  Accepted = 2,            // Accepted by Studio
  Scheduled = 3,           // Shoot scheduled (staff assigned)
  InProgress = 4,          // Shoot in progress
  AwaitingDelivery = 5,    // Shoot completed, waiting for delivery upload
  AwaitingConfirmation = 6, // Delivery uploaded, waiting for Agent confirmation
  Completed = 7,           // Completed
  Cancelled = 8,           // Cancelled
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

export enum ListingMediaRole {
  Cover = 0,
  Gallery = 1,
  FloorPlan = 2,
  Video = 3,
  VrPreview = 4,
}
