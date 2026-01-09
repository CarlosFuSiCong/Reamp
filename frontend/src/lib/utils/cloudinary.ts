/**
 * Cloudinary Image Transformation Utility
 * 
 * Provides type-safe URL transformation for Cloudinary-hosted images.
 * Similar to C# Cloudinary SDK but adapted for frontend use.
 */

export interface CloudinaryTransformOptions {
  width?: number;
  height?: number;
  crop?: "scale" | "fill" | "fit" | "limit" | "pad" | "thumb" | "crop";
  quality?: "auto" | "auto:best" | "auto:good" | "auto:eco" | "auto:low" | number;
  format?: "auto" | "jpg" | "png" | "webp" | "avif" | "gif";
  gravity?: "auto" | "face" | "faces" | "center" | "north" | "south" | "east" | "west";
  aspectRatio?: string; // e.g., "16:9", "4:3", "1:1"
  dpr?: "auto" | number; // Device Pixel Ratio
  flags?: string[]; // e.g., ["progressive", "lossy"]
  effect?: string; // e.g., "blur:300", "grayscale"
}

/**
 * Preset transformation configurations for common use cases
 */
export const CloudinaryPresets = {
  // Thumbnails for galleries and cards
  thumbnail: {
    width: 400,
    height: 300,
    crop: "fill" as const,
    quality: "auto" as const,
    format: "auto" as const,
    gravity: "auto" as const,
  },
  
  // Small thumbnails for lists
  thumbnailSmall: {
    width: 200,
    height: 150,
    crop: "fill" as const,
    quality: "auto:good" as const,
    format: "auto" as const,
    gravity: "auto" as const,
  },

  // Square thumbnails for avatars or grid displays
  thumbnailSquare: {
    width: 300,
    height: 300,
    crop: "fill" as const,
    quality: "auto" as const,
    format: "auto" as const,
    gravity: "face" as const,
  },

  // Optimized for listing cards
  listingCard: {
    width: 600,
    height: 400,
    crop: "fill" as const,
    quality: "auto" as const,
    format: "auto" as const,
    gravity: "auto" as const,
    // Remove aspectRatio to avoid conflicts with width/height
  },

  // Large display for lightbox/modal
  lightbox: {
    width: 1920,
    height: 1080,
    crop: "limit" as const,
    quality: "auto:best" as const,
    format: "auto" as const,
  },

  // Medium display for main gallery view
  galleryMain: {
    width: 1200,
    crop: "scale" as const,
    quality: "auto" as const,
    format: "auto" as const,
  },

  // Hero images for banners
  hero: {
    width: 1920,
    height: 600,
    crop: "fill" as const,
    quality: "auto:best" as const,
    format: "auto" as const,
    gravity: "auto" as const,
  },

  // Avatar images
  avatar: {
    width: 200,
    height: 200,
    crop: "fill" as const,
    quality: "auto" as const,
    format: "auto" as const,
    gravity: "face" as const,
  },

  // Floor plans - preserve quality and aspect ratio
  floorPlan: {
    width: 1200,
    crop: "limit" as const,
    quality: "auto:best" as const,
    format: "auto" as const,
  },

  // Optimized for mobile devices
  mobile: {
    width: 800,
    crop: "scale" as const,
    quality: "auto:good" as const,
    format: "auto" as const,
    dpr: "auto" as const,
  },
} as const;

/**
 * Extracts Cloudinary public_id and cloud name from a Cloudinary URL
 * Handles folders (e.g., reamp-dev/image_id) in the public_id
 */
export function parseCloudinaryUrl(url: string): { cloudName: string; publicId: string; extension?: string } | null {
  if (!url || !url.includes("cloudinary.com")) {
    return null;
  }

  try {
    // Remove query parameters first
    const urlWithoutQuery = url.split('?')[0];
    
    // Pattern: https://res.cloudinary.com/{cloud_name}/{resource_type}/upload/{rest}
    const basePattern = /cloudinary\.com\/([^/]+)\/(image|video)\/upload\/(.+)$/;
    const baseMatch = urlWithoutQuery.match(basePattern);
    
    if (!baseMatch) {
      return null;
    }
    
    const cloudName = baseMatch[1];
    let remainingPath = baseMatch[3]; // Everything after /upload/
    
    // Extract and preserve file extension
    const extensionMatch = remainingPath.match(/\.(jpg|jpeg|png|gif|webp|avif|mp4|mov|avi|webm)$/i);
    const extension = extensionMatch ? extensionMatch[0] : undefined;
    
    // Remove file extension from path
    if (extension) {
      remainingPath = remainingPath.slice(0, -extension.length);
    }
    
    // Remove version prefix if exists (e.g., v1234567890/)
    remainingPath = remainingPath.replace(/^v\d+\//, '');
    
    // Remove transformation parameters (e.g., w_800,c_fill/)
    // Transformations are at the beginning and separated by commas, followed by /
    // Format: w_800,c_fill,q_auto/folder/image or w_800,c_fill,q_auto/image
    const transformPattern = /^[^/]+,[^/]+\//;
    if (transformPattern.test(remainingPath)) {
      remainingPath = remainingPath.replace(transformPattern, '');
    }
    
    // Also handle single transformation parameter (e.g., w_800/)
    const singleTransformPattern = /^[a-z]_[^/]+\//;
    while (singleTransformPattern.test(remainingPath)) {
      remainingPath = remainingPath.replace(singleTransformPattern, '');
    }
    
    return {
      cloudName,
      publicId: remainingPath,
      extension,
    };
  } catch (error) {
    console.error('Error parsing Cloudinary URL:', error);
    return null;
  }
}

/**
 * Builds transformation string from options
 */
function buildTransformationString(options: CloudinaryTransformOptions): string {
  const parts: string[] = [];

  if (options.width) parts.push(`w_${options.width}`);
  if (options.height) parts.push(`h_${options.height}`);
  if (options.crop) parts.push(`c_${options.crop}`);
  if (options.aspectRatio) {
    // Cloudinary expects "ar_16:9" format, not "ar_16_9"
    parts.push(`ar_${options.aspectRatio.replace('_', ':')}`);
  }
  if (options.gravity) parts.push(`g_${options.gravity}`);
  if (options.quality) parts.push(`q_${options.quality}`);
  if (options.format) parts.push(`f_${options.format}`);
  if (options.dpr) parts.push(`dpr_${options.dpr}`);
  if (options.effect) parts.push(`e_${options.effect}`);
  if (options.flags && options.flags.length > 0) {
    parts.push(`fl_${options.flags.join(".")}`);
  }

  return parts.join(",");
}

/**
 * Transforms a Cloudinary URL with the specified options
 * 
 * @param url - Original Cloudinary URL
 * @param options - Transformation options
 * @returns Transformed Cloudinary URL
 * 
 * @example
 * ```ts
 * const url = transformCloudinaryUrl(
 *   "https://res.cloudinary.com/demo/image/upload/sample.jpg",
 *   { width: 1000, crop: "scale", quality: "auto", format: "auto" }
 * );
 * ```
 */
export function transformCloudinaryUrl(
  url: string | null | undefined,
  options: CloudinaryTransformOptions
): string {
  // Return placeholder if no URL provided
  if (!url) {
    return "/placeholder-property.jpg";
  }

  // If not a Cloudinary URL, return as-is
  const parsed = parseCloudinaryUrl(url);
  if (!parsed) {
    return url;
  }

  const { cloudName, publicId, extension } = parsed;
  const transformString = buildTransformationString(options);

  // Build new URL with transformations and preserve extension
  // If extension exists, append it; otherwise Cloudinary will use original format
  const fullPublicId = extension ? `${publicId}${extension}` : publicId;
  const transformedUrl = `https://res.cloudinary.com/${cloudName}/image/upload/${transformString}/${fullPublicId}`;
  
  // Debug logging in development
  if (process.env.NODE_ENV === 'development') {
    console.debug('[Cloudinary] Transform URL:', {
      original: url,
      parsed: { cloudName, publicId, extension },
      transformString,
      result: transformedUrl
    });
  }
  
  return transformedUrl;
}

/**
 * Applies a preset transformation to a Cloudinary URL
 * 
 * @param url - Original Cloudinary URL
 * @param presetName - Name of the preset from CloudinaryPresets
 * @returns Transformed Cloudinary URL
 * 
 * @example
 * ```ts
 * const thumbnailUrl = applyCloudinaryPreset(imageUrl, "thumbnail");
 * const lightboxUrl = applyCloudinaryPreset(imageUrl, "lightbox");
 * ```
 */
export function applyCloudinaryPreset(
  url: string | null | undefined,
  presetName: keyof typeof CloudinaryPresets
): string {
  const preset = CloudinaryPresets[presetName];
  return transformCloudinaryUrl(url, preset);
}

/**
 * Generates multiple transformed URLs for responsive images (srcset)
 * 
 * @param url - Original Cloudinary URL
 * @param widths - Array of widths to generate
 * @param options - Base transformation options
 * @returns Array of { url, width } objects for srcset
 */
export function generateResponsiveSrcSet(
  url: string | null | undefined,
  widths: number[] = [320, 640, 960, 1280, 1920],
  options: CloudinaryTransformOptions = {}
): Array<{ url: string; width: number }> {
  if (!url) {
    return widths.map((width) => ({ url: "/placeholder-property.jpg", width }));
  }

  return widths.map((width) => ({
    url: transformCloudinaryUrl(url, { ...options, width }),
    width,
  }));
}

/**
 * Helper to get optimized image URL with common defaults
 * Same as transformCloudinaryUrl but with sensible defaults
 * 
 * @param url - Original Cloudinary URL
 * @param width - Desired width
 * @param options - Additional options
 */
export function getOptimizedImageUrl(
  url: string | null | undefined,
  width?: number,
  options: Partial<CloudinaryTransformOptions> = {}
): string {
  return transformCloudinaryUrl(url, {
    width,
    crop: "scale",
    quality: "auto",
    format: "auto",
    ...options,
  });
}

