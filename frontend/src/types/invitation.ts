export enum InvitationType {
  Agency = 1,
  Studio = 2,
}

export enum InvitationStatus {
  Pending = 1,
  Accepted = 2,
  Rejected = 3,
  Cancelled = 4,
  Expired = 5,
}

export enum AgencyRole {
  Owner = 3,
  Manager = 2,
  Member = 1,
}

export enum StudioRole {
  Owner = 3,
  Manager = 2,
  Member = 1,
}

export interface InvitationListDto {
  id: string;
  type: InvitationType;
  targetEntityName: string;
  targetRoleName: string;
  status: InvitationStatus;
  expiresAtUtc: string;
  createdAtUtc: string;
  isExpired: boolean;
}

export interface InvitationDetailDto {
  id: string;
  type: InvitationType;
  targetEntityId: string;
  targetEntityName: string;
  targetBranchId?: string;
  targetBranchName?: string;
  inviteeEmail: string;
  targetRoleValue: number;
  targetRoleName: string;
  status: InvitationStatus;
  invitedBy: string;
  invitedByName: string;
  expiresAtUtc: string;
  createdAtUtc: string;
  respondedAtUtc?: string;
  isExpired: boolean;
}

export interface SendAgencyInvitationDto {
  inviteeEmail: string;
  targetRole: AgencyRole;
  agencyBranchId?: string;
}

export interface SendStudioInvitationDto {
  inviteeEmail: string;
  targetRole: StudioRole;
}
