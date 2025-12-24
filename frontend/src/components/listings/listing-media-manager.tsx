"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus, Trash2, Eye, EyeOff, Image as ImageIcon, Star } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Badge } from "@/components/ui/badge";
import { deliveriesApi, listingsApi } from "@/lib/api";
import { ListingMediaRole, DeliveryStatus } from "@/types";
import { toast } from "sonner";
import { ConfirmDialog } from "@/components/shared";
import { applyCloudinaryPreset } from "@/lib/utils/cloudinary";

interface ListingMediaManagerProps {
  listingId: string;
}

export function ListingMediaManager({ listingId }: ListingMediaManagerProps) {
  const queryClient = useQueryClient();
  const [selectedDeliveryId, setSelectedDeliveryId] = useState<string>("");
  const [selectedMediaIds, setSelectedMediaIds] = useState<Set<string>>(new Set());
  const [deleteConfirm, setDeleteConfirm] = useState<{ open: boolean; mediaId?: string }>({
    open: false,
  });

  // Fetch deliveries for this listing
  const { data: deliveries, isLoading: deliveriesLoading } = useQuery({
    queryKey: ["deliveries", "listing", listingId],
    queryFn: () => deliveriesApi.getByListingId(listingId),
  });

  // Fetch selected delivery details
  const { data: selectedDelivery } = useQuery({
    queryKey: ["delivery", selectedDeliveryId],
    queryFn: () => deliveriesApi.getById(selectedDeliveryId),
    enabled: !!selectedDeliveryId,
  });

  // Mutation to add media to listing
  const addMediaMutation = useMutation({
    mutationFn: (data: { mediaAssetId: string; role: ListingMediaRole }) =>
      listingsApi.addMedia(listingId, data),
    onSuccess: () => {
      toast.success("Media added to listing");
      queryClient.invalidateQueries({ queryKey: ["listing", listingId] });
      setSelectedMediaIds(new Set());
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to add media");
    },
  });

  // Mutation to remove media from listing
  const removeMediaMutation = useMutation({
    mutationFn: (mediaRefId: string) => listingsApi.removeMedia(listingId, mediaRefId),
    onSuccess: () => {
      toast.success("Media removed from listing");
      queryClient.invalidateQueries({ queryKey: ["listing", listingId] });
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to remove media");
    },
  });

  const handleAddSelectedMedia = () => {
    if (selectedMediaIds.size === 0) {
      toast.error("Please select at least one media file");
      return;
    }

    // Add all selected media as Gallery images
    selectedMediaIds.forEach((mediaAssetId) => {
      addMediaMutation.mutate({
        mediaAssetId,
        role: ListingMediaRole.Gallery,
      });
    });
  };

  const handleRemoveMedia = () => {
    if (deleteConfirm.mediaId) {
      removeMediaMutation.mutate(deleteConfirm.mediaId);
      setDeleteConfirm({ open: false });
    }
  };

  const publishedDeliveries =
    deliveries?.filter((d) => d.status === DeliveryStatus.Published) || [];

  return (
    <div className="space-y-6">
      {/* Select Delivery */}
      <Card>
        <CardHeader>
          <CardTitle>Add Media from Deliveries</CardTitle>
          <CardDescription>
            Select a delivery package to add photos and videos to this listing
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <Select value={selectedDeliveryId} onValueChange={setSelectedDeliveryId}>
              <SelectTrigger>
                <SelectValue placeholder="Select a delivery package" />
              </SelectTrigger>
              <SelectContent>
                {publishedDeliveries.length === 0 ? (
                  <SelectItem value="none" disabled>
                    No published deliveries available
                  </SelectItem>
                ) : (
                  publishedDeliveries.map((delivery) => (
                    <SelectItem key={delivery.id} value={delivery.id}>
                      {delivery.title} ({delivery.itemCount} files)
                    </SelectItem>
                  ))
                )}
              </SelectContent>
            </Select>
          </div>

          {/* Media Grid from Selected Delivery */}
          {selectedDelivery && selectedDelivery.items.length > 0 && (
            <>
              <div className="grid grid-cols-4 gap-4">
                {selectedDelivery.items.map((item) => (
                  <button
                    key={item.id}
                    type="button"
                    onClick={() => {
                      setSelectedMediaIds((prev) => {
                        const newSet = new Set(prev);
                        if (newSet.has(item.mediaAssetId)) {
                          newSet.delete(item.mediaAssetId);
                        } else {
                          newSet.add(item.mediaAssetId);
                        }
                        return newSet;
                      });
                    }}
                    className={`
                      relative aspect-square rounded-lg overflow-hidden border-2 transition-all
                      ${
                        selectedMediaIds.has(item.mediaAssetId)
                          ? "border-blue-600 ring-2 ring-blue-600"
                          : "border-gray-200 hover:border-blue-500"
                      }
                    `}
                  >
                    <div className="w-full h-full bg-gray-100 flex items-center justify-center">
                      {item.thumbnailUrl ? (
                        <img
                          src={applyCloudinaryPreset(item.thumbnailUrl, "thumbnailSquare")}
                          alt="Media"
                          className="w-full h-full object-cover"
                        />
                      ) : (
                        <ImageIcon className="h-8 w-8 text-gray-400" />
                      )}
                    </div>
                    {selectedMediaIds.has(item.mediaAssetId) && (
                      <div className="absolute top-2 right-2 bg-blue-600 rounded-full p-1">
                        <Star className="h-4 w-4 text-white fill-white" />
                      </div>
                    )}
                  </button>
                ))}
              </div>

              <Button
                type="button"
                onClick={handleAddSelectedMedia}
                disabled={selectedMediaIds.size === 0 || addMediaMutation.isPending}
              >
                <Plus className="mr-2 h-4 w-4" />
                Add {selectedMediaIds.size > 0 && `(${selectedMediaIds.size})`} to Listing
              </Button>
            </>
          )}
        </CardContent>
      </Card>

      {/* Current Listing Media */}
      <Card>
        <CardHeader>
          <CardTitle>Current Media</CardTitle>
          <CardDescription>Media files currently attached to this listing</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="text-center py-8 text-muted-foreground">
            <ImageIcon className="mx-auto h-12 w-12 mb-4 opacity-50" />
            <p>Media management integration coming soon</p>
            <p className="text-sm mt-2">
              Will display: media grid, role badges, visibility toggles, and sorting
            </p>
          </div>
        </CardContent>
      </Card>

      <ConfirmDialog
        open={deleteConfirm.open}
        onOpenChange={(open) => setDeleteConfirm({ ...deleteConfirm, open })}
        title="Remove Media"
        description="Are you sure you want to remove this media from the listing?"
        onConfirm={handleRemoveMedia}
        confirmText="Remove"
        variant="destructive"
      />
    </div>
  );
}
