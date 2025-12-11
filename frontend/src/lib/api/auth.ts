import apiClient from "@/lib/api-client";
import { LoginDto, RegisterDto, LoginResponse, UserProfile } from "@/types";

export const authApi = {
  // Login
  async login(data: LoginDto): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>("/api/auth/login", data);
    return response.data;
  },

  // Register
  async register(data: RegisterDto): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>("/api/auth/register", data);
    return response.data;
  },

  // Refresh token
  async refresh(): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>("/api/auth/refresh");
    return response.data;
  },

  // Logout
  async logout(): Promise<void> {
    await apiClient.post("/api/auth/logout");
  },

  // Get current user profile
  async getCurrentUser(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>("/api/profiles/me");
    return response.data;
  },
};
