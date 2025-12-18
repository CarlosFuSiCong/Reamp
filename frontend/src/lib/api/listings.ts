import apiClient from "@/lib/api-client";
import type { Listing, PagedResponse, AddMediaDto, ReorderMediaDto, SetMediaVisibilityDto } from "@/types";

export const listingsApi = {
  async list(params: {
    agencyId?: string;
    status?: string;
    type?: string;
    property?: string;
    keyword?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    desc?: boolean;
  }): Promise<PagedResponse<Listing>> {
    const response = await apiClient.get<PagedResponse<Listing>>("/api/listings", {
      params,
    });
    return response.data;
  },

  async getById(id: string): Promise<Listing> {
    const response = await apiClient.get<Listing>(`/api/listings/${id}`);
    return response.data;
  },

  async create(data: Partial<Listing>): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>("/api/listings", data);
    return response.data;
  },

  async update(id: string, data: Partial<Listing>): Promise<void> {
    await apiClient.put(`/api/listings/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/listings/${id}`);
  },

  async publish(id: string): Promise<void> {
    await apiClient.post(`/api/listings/${id}/publish`);
  },

  async archive(id: string): Promise<void> {
    await apiClient.post(`/api/listings/${id}/archive`);
  },

  // ===== Media Management =====

  /**
   * Add media to listing
   */
  async addMedia(listingId: string, dto: AddMediaDto): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>(
      `/api/listings/${listingId}/media`,
      dto
    );
    return response.data;
  },

  /**
   * Remove media from listing
   */
  async removeMedia(listingId: string, mediaRefId: string): Promise<void> {
    await apiClient.delete(`/api/listings/${listingId}/media/${mediaRefId}`);
  },

  /**
   * Reorder media
   */
  async reorderMedia(listingId: string, dto: ReorderMediaDto): Promise<void> {
    await apiClient.put(`/api/listings/${listingId}/media/reorder`, dto);
  },

  /**
   * Set media visibility
   */
  async setMediaVisibility(listingId: string, dto: SetMediaVisibilityDto): Promise<void> {
    await apiClient.put(`/api/listings/${listingId}/media/visibility`, dto);
  },
};
