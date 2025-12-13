import apiClient from "@/lib/api-client";

export interface MediaUploadResponse {
  id: string;
  publicUrl: string;  // Match backend PascalCase -> camelCase from JSON serialization
}

export const mediaApi = {
  async upload(file: File, onProgress?: (progress: number) => void): Promise<MediaUploadResponse> {
    const formData = new FormData();
    formData.append("file", file);

    const response = await apiClient.post<MediaUploadResponse>("/api/media/upload", formData, {
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(progress);
        }
      },
    });

    return response.data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/media/${id}`);
  },

  async getUrl(id: string): Promise<string> {
    // Get media asset details and return its publicUrl
    const response = await apiClient.get<{ publicUrl: string }>(`/api/media/${id}`);
    return response.data.publicUrl;
  },
};
