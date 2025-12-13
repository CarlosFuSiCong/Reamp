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
      const originalData = response.data;
      response.data = response.data.data as typeof response.data;
      
      if (process.env.NEXT_PUBLIC_ENABLE_DEBUG === "true") {
        console.log("API Response unwrapped:", {
          original: originalData,
          unwrapped: response.data,
        });
      }
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
      const originalRequest = error.config;

      if (!originalRequest) {
        return Promise.reject(error);
      }

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
