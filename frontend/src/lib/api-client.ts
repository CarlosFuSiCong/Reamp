import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from "axios";
import { ApiError, ApiResponse } from "@/types";
import { getCookie } from "@/lib/utils/cookies";

const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
const API_TIMEOUT = Number(process.env.NEXT_PUBLIC_API_TIMEOUT) || 30000;

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: API_URL,
  timeout: API_TIMEOUT,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

// Request interceptor
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    if (typeof window !== "undefined") {
      const token = getCookie("accessToken");
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }

    if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
      console.log("API Request:", {
        method: config.method,
        url: config.url,
        data: config.data,
      });
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
apiClient.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
      console.log("API Response:", {
        status: response.status,
        data: response.data,
      });
    }

    // Unwrap ApiResponse and return only the data field
    if (response.data && typeof response.data === "object" && "data" in response.data) {
      response.data = response.data.data as typeof response.data;
    }

    return response;
  },
  async (error: AxiosError<ApiResponse>) => {
    if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
      console.error("API Error:", {
        status: error.response?.status,
        message: error.response?.data?.message,
        errors: error.response?.data?.errors,
      });
    }

    // Handle 401 Unauthorized - token expired
    if (error.response?.status === 401) {
      try {
        await axios.post(
          `${API_URL}/api/auth/refresh`,
          {},
          { withCredentials: true }
        );

        if (error.config) {
          return apiClient.request(error.config);
        }
      } catch (refreshError) {
        if (typeof window !== "undefined") {
          window.location.href = "/login";
        }
        return Promise.reject(refreshError);
      }
    }

    // Handle other errors
    const apiError: ApiError = {
      message: error.response?.data?.message || error.message || "An unexpected error occurred",
      errors: error.response?.data?.errors,
      statusCode: error.response?.status || 500,
    };

    return Promise.reject(apiError);
  }
);

export default apiClient;
