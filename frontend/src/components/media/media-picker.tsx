"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Check, Loader2, Search, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Badge } from "@/components/ui/badge";
import { mediaApi } from "@/lib/api";
import { ChunkedUpload } from "./chunked-upload";
import type { MediaAssetDetailDto, MediaAssetListDto } from "@/types";
import { cn } from "@/lib/utils";

interface MediaPickerProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  studioId: string;
  onSelect: (assets: MediaAssetDetailDto[]) => void;
  multiple?: boolean;
  selectedAssetIds?: string[];
}

export function MediaPicker({
  open,
  onOpenChange,
  studioId,
  onSelect,
  multiple = true,
  selectedAssetIds = [],
}: MediaPickerProps) {
  const [activeTab, setActiveTab] = useState<"browse" | "upload">("browse");
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedIds, setSelectedIds] = useState<Set<string>>(
    new Set(selectedAssetIds)
  );
  const [page, setPage] = useState(1);

  // Fetch media assets
  const { data, isLoading } = useQuery({
    queryKey: ["media", "studio", studioId, page],
    queryFn: () =>
      mediaApi.listByStudio({
        studioId,
        page,
        pageSize: 20,
      }),
    enabled: open && !!studioId,
  });

  const toggleSelection = (assetId: string) => {
    setSelectedIds((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(assetId)) {
        newSet.delete(assetId);
      } else {
        if (!multiple) {
          newSet.clear();
        }
        newSet.add(assetId);
      }
      return newSet;
    });
  };

  const handleUploadComplete = (asset: MediaAssetDetailDto) => {
    // Auto-select uploaded asset
    if (!multiple) {
      setSelectedIds(new Set([asset.id]));
    } else {
      setSelectedIds((prev) => new Set(prev).add(asset.id));
    }

    // Switch to browse tab
    setActiveTab("browse");
  };

  const handleConfirm = async () => {
    // Fetch full details for selected assets
    const selectedAssets = await Promise.all(
      Array.from(selectedIds).map((id) => mediaApi.getById(id))
    );
    onSelect(selectedAssets);
    onOpenChange(false);
  };

  const filteredAssets = data?.items.filter((asset) =>
    asset.originalFileName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[80vh]">
        <DialogHeader>
          <DialogTitle>Select Media</DialogTitle>
        </DialogHeader>

        <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as any)}>
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="browse">Browse Media</TabsTrigger>
            <TabsTrigger value="upload">Upload New</TabsTrigger>
          </TabsList>

          <TabsContent value="browse" className="space-y-4">
            {/* Search */}
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Search media..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-9"
              />
            </div>

            {/* Media Grid */}
            <ScrollArea className="h-[400px]">
              {isLoading && (
                <div className="flex items-center justify-center h-64">
                  <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
                </div>
              )}

              {!isLoading && filteredAssets && filteredAssets.length === 0 && (
                <div className="flex flex-col items-center justify-center h-64 text-muted-foreground">
                  <p>No media found</p>
                  <Button
                    variant="link"
                    onClick={() => setActiveTab("upload")}
                  >
                    Upload some files
                  </Button>
                </div>
              )}

              {!isLoading && filteredAssets && filteredAssets.length > 0 && (
                <div className="grid grid-cols-4 gap-4 p-2">
                  {filteredAssets.map((asset) => (
                    <MediaCard
                      key={asset.id}
                      asset={asset}
                      isSelected={selectedIds.has(asset.id)}
                      onToggle={() => toggleSelection(asset.id)}
                    />
                  ))}
                </div>
              )}
            </ScrollArea>

            {/* Pagination */}
            {data && Math.ceil((data.total || 0) / (data.pageSize || 20)) > 1 && (
              <div className="flex items-center justify-between">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={page === 1}
                  onClick={() => setPage((p) => p - 1)}
                >
                  Previous
                </Button>
                <span className="text-sm text-muted-foreground">
                  Page {page} of {Math.ceil((data.total || 0) / (data.pageSize || 20))}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={page >= Math.ceil((data.total || 0) / (data.pageSize || 20))}
                  onClick={() => setPage((p) => p + 1)}
                >
                  Next
                </Button>
              </div>
            )}
          </TabsContent>

          <TabsContent value="upload">
            <ChunkedUpload
              ownerStudioId={studioId}
              onUploadComplete={handleUploadComplete}
              maxFiles={multiple ? 10 : 1}
            />
          </TabsContent>
        </Tabs>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleConfirm} disabled={selectedIds.size === 0}>
            Select {selectedIds.size > 0 && `(${selectedIds.size})`}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

interface MediaCardProps {
  asset: MediaAssetListDto;
  isSelected: boolean;
  onToggle: () => void;
}

function MediaCard({ asset, isSelected, onToggle }: MediaCardProps) {
  return (
    <button
      onClick={onToggle}
      className={cn(
        "relative aspect-square rounded-lg overflow-hidden border-2 transition-all",
        "hover:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500",
        isSelected ? "border-blue-600 ring-2 ring-blue-600" : "border-gray-200"
      )}
    >
      {/* Thumbnail */}
      <div className="w-full h-full bg-gray-100 flex items-center justify-center">
        {asset.thumbnailUrl ? (
          <img
            src={asset.thumbnailUrl}
            alt={asset.originalFileName}
            className="w-full h-full object-cover"
          />
        ) : (
          <div className="text-4xl text-gray-400">📄</div>
        )}
      </div>

      {/* Selection Indicator */}
      {isSelected && (
        <div className="absolute top-2 right-2 bg-blue-600 rounded-full p-1">
          <Check className="h-4 w-4 text-white" />
        </div>
      )}

      {/* File Name */}
      <div className="absolute bottom-0 left-0 right-0 bg-black/60 text-white p-2">
        <p className="text-xs truncate">{asset.originalFileName}</p>
      </div>
    </button>
  );
}
