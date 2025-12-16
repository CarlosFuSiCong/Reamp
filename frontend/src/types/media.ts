// Media types

export enum MediaProvider {
  Cloudinary = 0,
  LocalStorage = 1,
}

export enum MediaResourceType {
  Image = 0,
  Video = 1,
  Audio = 2,
  Document = 3,
  Other = 4,
}

export enum MediaProcessStatus {
  Uploaded = 0,
  Processing = 1,
  Ready = 2,
  Failed = 3,
}

export enum ListingMediaRole {
  Cover = 0,
  Gallery = 1,
  FloorPlan = 2,
  Video = 3,
  VrPreview = 4,
}

// DTOs
export interface MediaAssetDetailDto {
  id: string;
  originalFileName: string;
  storedFileName: string;
  mediaProvider: MediaProvider;
  externalId?: string;
  resourceType: MediaResourceType;
  processStatus: MediaProcessStatus;
  sizeBytes: number;
  contentType: string;
  checksumSha256?: string;
  ownerStudioId?: string;
  uploadedBy: string;
  uploadedAt: string;
  variants: MediaVariantDto[];
}

export interface MediaVariantDto {
  id: string;
  variantName: string;
  url: string;
  width?: number;
  height?: number;
  sizeBytes: number;
  format?: string;
}

export interface MediaAssetListDto {
  id: string;
  originalFileName: string;
  resourceType: MediaResourceType;
  processStatus: MediaProcessStatus;
  ownerStudioId?: string;
  uploadedAt: string;
  thumbnailUrl?: string;
}

// Chunked Upload DTOs
export interface InitiateChunkedUploadDto {
  ownerStudioId: string;
  fileName: string;
  contentType: string;
  totalSize: number;
  totalChunks: number;
  description?: string;
}

export interface UploadSessionDto {
  sessionId: string;
  fileName: string;
  totalSize: number;
  totalChunks: number;
  uploadedChunks: number;
  progress: number;
  createdAtUtc: string;
  completedAtUtc?: string;
}

// Listing Media DTOs
export interface AddMediaDto {
  mediaAssetId: string;
  role?: ListingMediaRole;
}

export interface ReorderMediaDto {
  mediaOrders: Record<string, number>;
}

export interface SetMediaVisibilityDto {
  mediaId: string;
  isVisible: boolean;
}

// Upload Progress Event
export interface UploadProgressEvent {
  fileName: string;
  progress: number;
  uploadedChunks: number;
  totalChunks: number;
  status: "uploading" | "completed" | "error" | "cancelled";
  error?: string;
}
