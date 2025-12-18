"use client";

import { useState } from "react";
import Image from "next/image";
import { Dialog, DialogContent } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight, X, Maximize2 } from "lucide-react";
import type { ListingMedia } from "@/types";

interface ImageGalleryProps {
  media: ListingMedia[];
  title: string;
}

export function ImageGallery({ media, title }: ImageGalleryProps) {
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [isLightboxOpen, setIsLightboxOpen] = useState(false);

  // Filter visible media and get cover first
  const visibleMedia = media
    ?.filter((m) => m.isVisible !== false)
    .sort((a, b) => {
      if (a.isCover) return -1;
      if (b.isCover) return 1;
      return a.sortOrder - b.sortOrder;
    }) || [];

  if (visibleMedia.length === 0) {
    return (
      <div className="aspect-[16/9] bg-gray-100 rounded-lg flex items-center justify-center">
        <p className="text-gray-400">No images available</p>
      </div>
    );
  }

  const currentImage = visibleMedia[selectedIndex];

  const handlePrevious = (e?: React.MouseEvent) => {
    e?.stopPropagation();
    setSelectedIndex((prev) => (prev === 0 ? visibleMedia.length - 1 : prev - 1));
  };

  const handleNext = (e?: React.MouseEvent) => {
    e?.stopPropagation();
    setSelectedIndex((prev) => (prev === visibleMedia.length - 1 ? 0 : prev + 1));
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "ArrowLeft") handlePrevious();
    if (e.key === "ArrowRight") handleNext();
    if (e.key === "Escape") setIsLightboxOpen(false);
  };

  return (
    <>
      <div className="space-y-4">
        {/* Main Image */}
        <div className="relative aspect-[16/9] bg-gray-100 rounded-lg overflow-hidden group">
          <Image
            src={currentImage.thumbnailUrl || "/placeholder-property.jpg"}
            alt={`${title} - Image ${selectedIndex + 1}`}
            fill
            className="object-cover"
            priority
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 80vw, 1200px"
          />

          {/* Navigation Arrows */}
          {visibleMedia.length > 1 && (
            <>
              <Button
                variant="secondary"
                size="icon"
                className="absolute left-4 top-1/2 -translate-y-1/2 opacity-0 group-hover:opacity-100 transition-opacity bg-white/90 hover:bg-white"
                onClick={handlePrevious}
              >
                <ChevronLeft className="h-5 w-5" />
              </Button>
              <Button
                variant="secondary"
                size="icon"
                className="absolute right-4 top-1/2 -translate-y-1/2 opacity-0 group-hover:opacity-100 transition-opacity bg-white/90 hover:bg-white"
                onClick={handleNext}
              >
                <ChevronRight className="h-5 w-5" />
              </Button>
            </>
          )}

          {/* Fullscreen Button */}
          <Button
            variant="secondary"
            size="icon"
            className="absolute right-4 bottom-4 opacity-0 group-hover:opacity-100 transition-opacity bg-white/90 hover:bg-white"
            onClick={() => setIsLightboxOpen(true)}
          >
            <Maximize2 className="h-5 w-5" />
          </Button>

          {/* Image Counter */}
          {visibleMedia.length > 1 && (
            <div className="absolute bottom-4 left-4 bg-black/60 text-white px-3 py-1 rounded-full text-sm">
              {selectedIndex + 1} / {visibleMedia.length}
            </div>
          )}
        </div>

        {/* Thumbnails */}
        {visibleMedia.length > 1 && (
          <div className="grid grid-cols-4 sm:grid-cols-6 md:grid-cols-8 gap-2">
            {visibleMedia.map((media, index) => (
              <button
                key={media.mediaAssetId}
                onClick={() => setSelectedIndex(index)}
                className={`relative aspect-square rounded-md overflow-hidden border-2 transition-all ${
                  index === selectedIndex
                    ? "border-blue-600 ring-2 ring-blue-600 ring-offset-2"
                    : "border-gray-200 hover:border-gray-400"
                }`}
              >
                <Image
                  src={media.thumbnailUrl || "/placeholder-property.jpg"}
                  alt={`Thumbnail ${index + 1}`}
                  fill
                  className="object-cover"
                  sizes="100px"
                />
              </button>
            ))}
          </div>
        )}
      </div>

      {/* Lightbox Modal */}
      <Dialog open={isLightboxOpen} onOpenChange={setIsLightboxOpen}>
        <DialogContent
          className="max-w-[95vw] max-h-[95vh] p-0 bg-black/95"
          onKeyDown={handleKeyDown}
        >
          <div className="relative w-full h-[90vh] flex items-center justify-center">
            <Image
              src={currentImage.thumbnailUrl || "/placeholder-property.jpg"}
              alt={`${title} - Image ${selectedIndex + 1}`}
              fill
              className="object-contain"
              sizes="95vw"
            />

            {/* Navigation */}
            {visibleMedia.length > 1 && (
              <>
                <Button
                  variant="ghost"
                  size="icon"
                  className="absolute left-4 top-1/2 -translate-y-1/2 bg-white/10 hover:bg-white/20 text-white"
                  onClick={handlePrevious}
                >
                  <ChevronLeft className="h-8 w-8" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="absolute right-4 top-1/2 -translate-y-1/2 bg-white/10 hover:bg-white/20 text-white"
                  onClick={handleNext}
                >
                  <ChevronRight className="h-8 w-8" />
                </Button>
              </>
            )}

            {/* Close Button */}
            <Button
              variant="ghost"
              size="icon"
              className="absolute top-4 right-4 bg-white/10 hover:bg-white/20 text-white"
              onClick={() => setIsLightboxOpen(false)}
            >
              <X className="h-6 w-6" />
            </Button>

            {/* Counter */}
            {visibleMedia.length > 1 && (
              <div className="absolute bottom-4 left-1/2 -translate-x-1/2 bg-black/60 text-white px-4 py-2 rounded-full">
                {selectedIndex + 1} / {visibleMedia.length}
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
}

