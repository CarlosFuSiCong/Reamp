import apiClient from "@/lib/api-client";
import type {
  DeliveryPackageDetailDto,
  DeliveryPackageListDto,
  CreateDeliveryPackageDto,
  UpdateDeliveryPackageDto,
  AddDeliveryItemDto,
  AddDeliveryAccessDto,
} from "@/types/delivery";

export const deliveriesApi = {
  /**
   * Create delivery package
   */
  create: async (dto: CreateDeliveryPackageDto): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.post<DeliveryPackageDetailDto>("/delivery", dto);
    return response.data;
  },

  /**
   * Get delivery package detail
   */
  getById: async (id: string): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.get<DeliveryPackageDetailDto>(`/delivery/${id}`);
    return response.data;
  },

  /**
   * Get packages by order
   */
  getByOrderId: async (orderId: string): Promise<DeliveryPackageListDto[]> => {
    const response = await apiClient.get<DeliveryPackageListDto[]>(`/delivery/order/${orderId}`);
    return response.data;
  },

  /**
   * Get packages by listing
   */
  getByListingId: async (listingId: string): Promise<DeliveryPackageListDto[]> => {
    const response = await apiClient.get<DeliveryPackageListDto[]>(`/delivery/listing/${listingId}`);
    return response.data;
  },

  /**
   * Update delivery package
   */
  update: async (id: string, dto: UpdateDeliveryPackageDto): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.put<DeliveryPackageDetailDto>(`/delivery/${id}`, dto);
    return response.data;
  },

  /**
   * Delete delivery package
   */
  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/delivery/${id}`);
  },

  /**
   * Add item to delivery package
   */
  addItem: async (id: string, dto: AddDeliveryItemDto): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.post<DeliveryPackageDetailDto>(`/delivery/${id}/items`, dto);
    return response.data;
  },

  /**
   * Remove item from delivery package
   */
  removeItem: async (id: string, itemId: string): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.delete<DeliveryPackageDetailDto>(`/delivery/${id}/items/${itemId}`);
    return response.data;
  },

  /**
   * Add access to delivery package
   */
  addAccess: async (id: string, dto: AddDeliveryAccessDto): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.post<DeliveryPackageDetailDto>(`/delivery/${id}/accesses`, dto);
    return response.data;
  },

  /**
   * Remove access from delivery package
   */
  removeAccess: async (id: string, accessId: string): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.delete<DeliveryPackageDetailDto>(`/delivery/${id}/accesses/${accessId}`);
    return response.data;
  },

  /**
   * Publish delivery package
   * NOTE: Publishing a delivery will automatically update the order status to AwaitingConfirmation
   */
  publish: async (id: string): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.post<DeliveryPackageDetailDto>(`/delivery/${id}/publish`);
    return response.data;
  },

  /**
   * Revoke delivery package
   */
  revoke: async (id: string): Promise<DeliveryPackageDetailDto> => {
    const response = await apiClient.post<DeliveryPackageDetailDto>(`/delivery/${id}/revoke`);
    return response.data;
  },
};
