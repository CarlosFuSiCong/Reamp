import apiClient from "@/lib/api-client";
import { LoginDto, RegisterDto, TokenResponse, UserProfile, PasswordPolicy } from "@/types";

export const authApi = {
  async getPasswordPolicy(): Promise<PasswordPolicy> {
    const response = await apiClient.get<PasswordPolicy>("/api/auth/password-policy");
    return response.data;
  },

  async login(data: LoginDto): Promise<TokenResponse> {
    const response = await apiClient.post<TokenResponse>("/api/auth/login", data);
    return response.data;
  },

  async register(data: RegisterDto): Promise<TokenResponse> {
    const response = await apiClient.post<TokenResponse>("/api/auth/register", data);
    return response.data;
  },

  async refresh(): Promise<TokenResponse> {
    const response = await apiClient.post<TokenResponse>("/api/auth/refresh");
    return response.data;
  },

  async logout(): Promise<void> {
    await apiClient.post("/api/auth/logout");
  },

  async getCurrentUser(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>("/api/profiles/me");
    return response.data;
  },
};
