export enum ApplicationType {
  Agency = 0,
  Studio = 1,
}

export enum ApplicationStatus {
  Pending = 0,
  UnderReview = 1,
  Approved = 2,
  Rejected = 3,
  Cancelled = 4,
}

export interface Address {
  street?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
}

export interface SubmitAgencyApplicationDto {
  organizationName: string;
  description?: string;
  contactEmail: string;
  contactPhone: string;
}

export interface SubmitStudioApplicationDto {
  organizationName: string;
  description?: string;
  contactEmail: string;
  contactPhone: string;
  address?: Address;
}

export interface ReviewApplicationDto {
  approved: boolean;
  notes?: string;
}

export interface ApplicationListDto {
  id: string;
  type: ApplicationType;
  status: ApplicationStatus;
  organizationName: string;
  contactEmail: string;
  applicantUserId: string;
  applicantEmail: string;
  applicantName: string;
  createdAtUtc: string;
  reviewedAtUtc?: string;
}

export interface ApplicationDetailDto {
  id: string;
  type: ApplicationType;
  status: ApplicationStatus;
  applicantUserId: string;
  applicantEmail: string;
  applicantName: string;
  organizationName: string;
  description?: string;
  contactEmail: string;
  contactPhone: string;
  address?: Address;
  createdOrganizationId?: string;
  reviewedBy?: string;
  reviewerName?: string;
  reviewedAtUtc?: string;
  reviewNotes?: string;
  createdAtUtc: string;
}
