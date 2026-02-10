import apiClient from "./axios.instance";
import type { ApiResponse, AuthUser, LoginRequest, RegisterRequest } from "@/types";

const AUTH_BASE = "/auth";

export const authService = {
  async login(data: LoginRequest): Promise<AuthUser> {
    const response = await apiClient.post<ApiResponse<AuthUser>>(
      `${AUTH_BASE}/login`,
      data
    );
    return response.data.data!;
  },

  async register(data: RegisterRequest): Promise<AuthUser> {
    const response = await apiClient.post<ApiResponse<AuthUser>>(
      `${AUTH_BASE}/register`,
      data
    );
    return response.data.data!;
  },

  async logout(): Promise<void> {
    await apiClient.post(`${AUTH_BASE}/logout`);
  },

  async refresh(): Promise<AuthUser> {
    const response = await apiClient.post<ApiResponse<AuthUser>>(
      `${AUTH_BASE}/refresh`
    );
    return response.data.data!;
  },

  async getCurrentUser(): Promise<AuthUser> {
    const response = await apiClient.get<ApiResponse<AuthUser>>(
      `${AUTH_BASE}/me`
    );
    return response.data.data!;
  },
};
