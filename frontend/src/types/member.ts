import { AgencyRole, StudioRole } from "./enums";

export interface AgencyMemberDto {
  id: string;
  userProfileId: string;
  displayName: string;
  email: string;
  avatarAssetId?: string;
  role: AgencyRole;
  agencyBranchId?: string;
  agencyBranchName?: string;
  joinedAtUtc: string;
}

export interface StudioMemberDto {
  id: string;
  userProfileId: string;
  displayName: string;
  email: string;
  avatarAssetId?: string;
  role: StudioRole;
  skills: string[];
  joinedAtUtc: string;
}

export interface UpdateAgencyMemberRoleDto {
  newRole: AgencyRole;
}

export interface UpdateStudioMemberRoleDto {
  newRole: StudioRole;
}
