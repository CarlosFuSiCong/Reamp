import apiClient from "../api-client";
import type {
  InitiateChunkedUploadDto,
  UploadSessionDto,
  MediaAssetDetailDto,
  MediaAssetListDto,
  PagedResponse,
} from "@/types";

export const mediaApi = {
  // ===== Chunked Upload =====
  
  /**
   * Initiate a chunked upload session
   */
  initiateChunkedUpload: async (
    dto: InitiateChunkedUploadDto
  ): Promise<UploadSessionDto> => {
    const response = await apiClient.post<UploadSessionDto>(
      "/api/media/chunked/initiate",
      dto
    );
    return response.data;
  },

  /**
   * Upload a single chunk
   */
  uploadChunk: async (
    sessionId: string,
    chunkIndex: number,
    chunkData: Blob
  ): Promise<UploadSessionDto> => {
    const formData = new FormData();
    formData.append("sessionId", sessionId);
    formData.append("chunkIndex", chunkIndex.toString());
    formData.append("chunk", chunkData);

    const response = await apiClient.post<UploadSessionDto>(
      "/api/media/chunked/upload",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
    return response.data;
  },

  /**
   * Complete chunked upload
   */
  completeChunkedUpload: async (
    sessionId: string
  ): Promise<MediaAssetDetailDto> => {
    const response = await apiClient.post<MediaAssetDetailDto>(
      `/api/media/chunked/complete/${sessionId}`
    );
    return response.data;
  },

  /**
   * Get upload session status
   */
  getUploadSessionStatus: async (
    sessionId: string
  ): Promise<UploadSessionDto> => {
    const response = await apiClient.get<UploadSessionDto>(
      `/api/media/chunked/status/${sessionId}`
    );
    return response.data;
  },

  /**
   * Cancel upload session
   */
  cancelUploadSession: async (sessionId: string): Promise<void> => {
    await apiClient.delete(`/api/media/chunked/cancel/${sessionId}`);
  },

  // ===== Media Asset Management =====

  /**
   * Get media asset by ID
   */
  getById: async (id: string): Promise<MediaAssetDetailDto> => {
    const response = await apiClient.get<MediaAssetDetailDto>(`/api/media/${id}`);
    return response.data;
  },

  /**
   * List media assets by studio
   */
  listByStudio: async (params: {
    studioId: string;
    page?: number;
    pageSize?: number;
  }): Promise<PagedResponse<MediaAssetListDto>> => {
    const response = await apiClient.get<PagedResponse<MediaAssetListDto>>(
      `/api/media/studio/${params.studioId}`,
      { params }
    );
    return response.data;
  },
};
