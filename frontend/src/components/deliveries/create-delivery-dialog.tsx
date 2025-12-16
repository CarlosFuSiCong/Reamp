"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { ChunkedUpload } from "@/components/media";
import { deliveriesApi } from "@/lib/api";
import { MediaAssetDetailDto } from "@/types";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

const deliverySchema = z.object({
  title: z.string().min(3, "Title must be at least 3 characters"),
  watermarkEnabled: z.boolean().default(false),
});

type DeliveryFormValues = z.infer<typeof deliverySchema>;

interface CreateDeliveryDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  orderId: string;
  listingId: string;
  studioId: string;
  onSuccess?: () => void;
}

export function CreateDeliveryDialog({
  open,
  onOpenChange,
  orderId,
  listingId,
  studioId,
  onSuccess,
}: CreateDeliveryDialogProps) {
  const [uploadedMedia, setUploadedMedia] = useState<MediaAssetDetailDto[]>([]);
  const [isCreating, setIsCreating] = useState(false);

  const { register, handleSubmit, formState: { errors }, reset } = useForm<DeliveryFormValues>({
    resolver: zodResolver(deliverySchema),
    defaultValues: {
      title: "",
      watermarkEnabled: false,
    },
  });

  const handleClose = () => {
    reset();
    setUploadedMedia([]);
    onOpenChange(false);
  };

  const onSubmit = async (data: DeliveryFormValues) => {
    if (uploadedMedia.length === 0) {
      toast.error("Please upload at least one media file");
      return;
    }

    setIsCreating(true);
    try {
      // Step 1: Create delivery package
      const delivery = await deliveriesApi.create({
        orderId,
        listingId,
        title: data.title,
        watermarkEnabled: data.watermarkEnabled,
      });

      // Step 2: Add media items
      for (const media of uploadedMedia) {
        await deliveriesApi.addItem(delivery.id, {
          mediaAssetId: media.id,
          variantName: "original",
        });
      }

      // Step 3: Publish delivery package
      await deliveriesApi.publish(delivery.id);

      toast.success("Delivery package created and published successfully");
      handleClose();
      onSuccess?.();
    } catch (error: any) {
      toast.error(error?.message || "Failed to create delivery package");
    } finally {
      setIsCreating(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Create Delivery Package</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Basic Info */}
          <div className="space-y-4">
            <div>
              <Label htmlFor="title">Title</Label>
              <Input
                id="title"
                placeholder="Property Photos - June 2024"
                {...register("title")}
              />
              {errors.title && (
                <p className="text-sm text-red-600 mt-1">{errors.title.message}</p>
              )}
            </div>

            <div className="flex items-center space-x-2">
              <Checkbox
                id="watermark"
                onCheckedChange={(checked) => {
                  const input = document.getElementById("watermark-hidden") as HTMLInputElement;
                  if (input) input.value = checked ? "true" : "false";
                }}
              />
              <input type="hidden" id="watermark-hidden" {...register("watermarkEnabled")} value="false" />
              <Label htmlFor="watermark" className="text-sm font-normal cursor-pointer">
                Enable watermark on images
              </Label>
            </div>
          </div>

          {/* Media Upload */}
          <div>
            <Label>Upload Media</Label>
            <div className="mt-2">
              <ChunkedUpload
                ownerStudioId={studioId}
                onUploadComplete={(asset) => {
                  setUploadedMedia((prev) => [...prev, asset]);
                  toast.success(`${asset.originalFileName} uploaded`);
                }}
                maxFiles={50}
                maxSizeMB={200}
              />
            </div>
          </div>

          {/* Uploaded Media Count */}
          {uploadedMedia.length > 0 && (
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <p className="text-sm text-blue-900">
                <strong>{uploadedMedia.length}</strong> file(s) uploaded and ready to deliver
              </p>
            </div>
          )}

          <DialogFooter>
            <Button type="button" variant="outline" onClick={handleClose} disabled={isCreating}>
              Cancel
            </Button>
            <Button type="submit" disabled={isCreating || uploadedMedia.length === 0}>
              {isCreating && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              {isCreating ? "Creating..." : "Create & Publish"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
