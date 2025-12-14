import apiClient from "@/lib/api-client";

// Backend ApiResponse wrapper structure
interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface MediaUploadResponse {
  id: string;
  publicUrl: string;
}

export const mediaApi = {
  async upload(file: File, onProgress?: (progress: number) => void): Promise<MediaUploadResponse> {
    const formData = new FormData();
    formData.append("file", file);

    const response = await apiClient.post<ApiResponse<MediaUploadResponse>>("/api/media/upload", formData, {
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(progress);
        }
      },
    });

    // Unwrap ApiResponse to get the actual data
    return response.data.data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/media/${id}`);
  },

  async getUrl(id: string): Promise<string> {
    const response = await apiClient.get<ApiResponse<{ publicUrl: string }>>(`/api/media/${id}`);
    return response.data.data.publicUrl;
  },
};
