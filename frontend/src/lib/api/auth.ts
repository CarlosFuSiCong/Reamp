import apiClient from "@/lib/api-client";
import { LoginDto, RegisterDto, UserInfoDto, UserProfile, PasswordPolicy } from "@/types";

export const authApi = {
  async getPasswordPolicy(): Promise<PasswordPolicy> {
    const response = await apiClient.get<PasswordPolicy>("/api/auth/password-policy");
    return response.data;
  },

  async login(data: LoginDto): Promise<UserInfoDto> {
    const response = await apiClient.post<UserInfoDto>("/api/auth/login", data);
    return response.data;
  },

  async register(data: RegisterDto): Promise<UserInfoDto> {
    const response = await apiClient.post<UserInfoDto>("/api/auth/register", data);
    return response.data;
  },

  async refresh(): Promise<void> {
    await apiClient.post("/api/auth/refresh");
  },

  async logout(): Promise<void> {
    await apiClient.post("/api/auth/logout");
  },

  async getCurrentUser(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>("/api/profiles/me");
    return response.data;
  },
};
