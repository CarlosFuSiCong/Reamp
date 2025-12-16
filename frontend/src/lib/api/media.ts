import apiClient from "@/lib/api-client";

// Backend ApiResponse wrapper structure
interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface MediaAssetDetailDto {
  id: string;
  ownerStudioId: string;
  studioName?: string;
  uploaderUserId: string;
  uploaderName?: string;
  mediaProvider: number;
  providerAssetId: string;
  resourceType: number;
  processStatus: number;
  contentType: string;
  sizeBytes: number;
  widthPx?: number;
  heightPx?: number;
  durationSeconds?: number;
  originalFileName: string;
  publicUrl: string;
  checksumSha256?: string;
  description?: string;
  tags?: string[];
  variants: Array<{
    variantName: string;
    transformedUrl: string;
    widthPx?: number;
    heightPx?: number;
    sizeBytes?: number;
  }>;
  createdAtUtc: string;
  updatedAtUtc: string;
}

// Simplified interface for upload response (only includes essential fields)
export interface MediaUploadResponse {
  id: string;
  publicUrl: string;
}

export const mediaApi = {
  async upload(file: File, onProgress?: (progress: number) => void): Promise<MediaUploadResponse> {
    const formData = new FormData();
    formData.append("file", file);

    const response = await apiClient.post<ApiResponse<MediaUploadResponse>>(
      "/api/media/upload",
      formData,
      {
        onUploadProgress: (progressEvent) => {
          if (onProgress && progressEvent.total) {
            const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
            onProgress(progress);
          }
        },
      }
    );

    // Unwrap ApiResponse to get the actual data
    return response.data.data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/media/${id}`);
  },

  async getById(id: string): Promise<MediaAssetDetailDto> {
    const response = await apiClient.get<ApiResponse<MediaAssetDetailDto>>(`/api/media/${id}`);
    return response.data.data;
  },

  async getUrl(id: string): Promise<string> {
    const response = await apiClient.get<ApiResponse<MediaAssetDetailDto>>(`/api/media/${id}`);
    return response.data.data.publicUrl;
  },
};
