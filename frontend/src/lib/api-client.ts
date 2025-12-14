import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from "axios";
import { ApiError, ApiResponse } from "@/types";

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

    if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
      console.log("API Request:", {
        method: config.method,
        url: config.url,
        data: config.data,
      });
      if (config.data) {
        console.log("Request data (detailed):", JSON.stringify(config.data, null, 2));
      }
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Track ongoing refresh to prevent race conditions
let isRefreshing = false;
let refreshSubscribers: ((error?: Error) => void)[] = [];

function subscribeTokenRefresh(cb: (error?: Error) => void) {
  refreshSubscribers.push(cb);
}

function onRefreshed(error?: Error) {
  refreshSubscribers.forEach((cb) => cb(error));
  refreshSubscribers = [];
}

// Response interceptor
apiClient.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
      console.log("API Response:", {
        status: response.status,
        data: response.data,
        url: response.config.url,
      });
    }

    // Unwrap ApiResponse and return only the data field
    if (response.data && typeof response.data === "object" && "data" in response.data) {
      const unwrapped = response.data.data;
      if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
        console.log("API Response unwrapped:", {
          original: response.data,
          unwrapped: unwrapped,
        });
      }
      response.data = unwrapped as typeof response.data;
    }

    return response;
  },
  async (error: AxiosError<ApiResponse>) => {
    if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
      console.error("API Error:", {
        status: error.response?.status,
        statusText: error.response?.statusText,
        message: error.response?.data?.message,
        errors: error.response?.data?.errors,
        url: error.config?.url,
        method: error.config?.method,
        data: error.config?.data,
        fullError: error.response?.data,
      });
    }

    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // Handle 401 Unauthorized - token expired
    if (error.response?.status === 401 && originalRequest) {
      // Prevent infinite loop - don't retry if already retried or if it's the refresh endpoint
      if (originalRequest._retry || originalRequest.url?.includes('/api/auth/refresh')) {
        if (typeof window !== "undefined") {
          window.location.href = "/login";
        }
        return Promise.reject(error);
      }

      // Mark this request as retried
      originalRequest._retry = true;

      // If already refreshing, queue this request
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          subscribeTokenRefresh((err?: Error) => {
            if (err) {
              reject(err);
            } else {
              resolve(apiClient.request(originalRequest));
            }
          });
        });
      }

      isRefreshing = true;

      try {
        await axios.post(
          `${API_URL}/api/auth/refresh`,
          {},
          { withCredentials: true }
        );

        isRefreshing = false;
        onRefreshed();
        return apiClient.request(originalRequest);
      } catch (refreshError) {
        isRefreshing = false;
        onRefreshed(refreshError as Error);
        
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
