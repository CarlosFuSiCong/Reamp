import { UserRole, UserStatus } from "./enums";

export interface User {
  id: string;
  email: string;
  role: UserRole;
  createdAt: string;
  updatedAt: string;
}

export interface UserProfile {
  id: string;
  applicationUserId: string;
  firstName: string;
  lastName: string;
  displayName: string;
  avatarAssetId?: string;
  role: UserRole;
  status: UserStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
}
