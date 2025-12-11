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

export interface LoginDto {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterDto {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}
