import { UserRole, UserStatus } from "./enums";

export interface User {
  id: string;
  email: string;
  role: UserRole;
  createdAt: string;
  updatedAt: string;
}

export interface UserInfoDto {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  displayName: string;
  role: UserRole;
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
  firstName?: string;
  lastName?: string;
  role: UserRole;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  tokenType: string;
}

export interface PasswordPolicy {
  requiredLength: number;
  requireNonAlphanumeric: boolean;
  requireDigit: boolean;
  requireLowercase: boolean;
  requireUppercase: boolean;
}
