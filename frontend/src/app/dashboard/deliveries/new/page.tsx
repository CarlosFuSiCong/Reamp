"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { PageHeader } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { DeliveryMediaUploader } from "@/components/media/delivery-media-uploader";
import { deliveriesApi, ordersApi } from "@/lib/api";
import { useProfile } from "@/lib/hooks";
import { MediaAssetDetailDto, OrderStatus } from "@/types";
import { toast } from "sonner";
import { ArrowLeft, Loader2, Save, Send } from "lucide-react";
import Link from "next/link";

const deliverySchema = z.object({
  orderId: z.string().min(1, "Please select an order"),
  title: z.string().min(3, "Title must be at least 3 characters"),
  watermarkEnabled: z.boolean(),
});

type DeliveryFormValues = z.infer<typeof deliverySchema>;

export default function NewDeliveryPage() {
  const router = useRouter();
  const { user } = useProfile();
  const [uploadedMedia, setUploadedMedia] = useState<MediaAssetDetailDto[]>([]);
  const [isSaving, setIsSaving] = useState(false);

  // Fetch orders that are ready for delivery upload (InProgress or AwaitingDelivery)
  const { data: ordersData } = useQuery({
    queryKey: ["orders", "ready-for-delivery", user?.studioId],
    queryFn: async () => {
      if (!user?.studioId) return { items: [], total: 0 };
      
      // Fetch both InProgress (4) and AwaitingDelivery (5) orders for this studio
      const [inProgressOrders, awaitingOrders] = await Promise.all([
        ordersApi.list({ 
          studioId: user.studioId,
          status: String(OrderStatus.InProgress), 
          page: 1, 
          pageSize: 100 
        }),
        ordersApi.list({ 
          studioId: user.studioId,
          status: String(OrderStatus.AwaitingDelivery), 
          page: 1, 
          pageSize: 100 
        }),
      ]);
      
      return {
        items: [...inProgressOrders.items, ...awaitingOrders.items],
        total: inProgressOrders.total + awaitingOrders.total,
      };
    },
    enabled: !!user?.studioId,
  });

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<DeliveryFormValues>({
    resolver: zodResolver(deliverySchema),
    defaultValues: {
      orderId: "",
      title: "",
      watermarkEnabled: false,
    },
  });

  const selectedOrderId = watch("orderId");
  const selectedOrder = ordersData?.items.find((o) => o.id === selectedOrderId);

  const handleSave = async (data: DeliveryFormValues, publish: boolean) => {
    if (uploadedMedia.length === 0) {
      toast.error("Please upload at least one media file");
      return;
    }

    if (!selectedOrder) {
      toast.error("Please select an order");
      return;
    }

    setIsSaving(true);
    try {
      // Step 1: Create delivery package
      const delivery = await deliveriesApi.create({
        orderId: data.orderId,
        listingId: selectedOrder.listingId,
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

      // Step 3: Publish or save as draft
      if (publish) {
        await deliveriesApi.publish(delivery.id);
        toast.success("Delivery published successfully");
      } else {
        toast.success("Delivery saved as draft. You can add more media later.");
      }

      router.push("/dashboard/deliveries");
    } catch (error: any) {
      toast.error(error?.message || "Failed to create delivery package");
    } finally {
      setIsSaving(false);
    }
  };

  if (!user?.studioId) {
    return (
      <div className="space-y-6">
        <PageHeader title="Create Delivery" description="Only Studio members can create deliveries" />
        <Card>
          <CardContent className="pt-6">
            <p className="text-muted-foreground text-center">
              You must be a member of a photography studio to create deliveries.
            </p>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Create Delivery"
        description="Upload media files and deliver to clients. You can save as draft and add more files later."
        action={
          <Link href="/dashboard/deliveries">
            <Button variant="outline">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back
            </Button>
          </Link>
        }
      />

      <form onSubmit={handleSubmit((data) => handleSave(data, false))} className="space-y-6">
        {/* Basic Info */}
        <Card>
          <CardHeader>
            <CardTitle>Delivery Information</CardTitle>
            <CardDescription>Select the order and provide delivery details</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <Label htmlFor="orderId">Order *</Label>
              <Select value={selectedOrderId} onValueChange={(value) => setValue("orderId", value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Select an order to deliver" />
                </SelectTrigger>
                <SelectContent>
                  {ordersData?.items.map((order) => (
                    <SelectItem key={order.id} value={order.id}>
                      Order #{order.id.slice(0, 8)} - {order.listingTitle || "Listing"} ({order.status === OrderStatus.InProgress ? "In Progress" : "Awaiting Delivery"})
                    </SelectItem>
                  ))}
                  {(!ordersData || ordersData.items.length === 0) && (
                    <SelectItem value="none" disabled>
                      No orders ready for delivery
                    </SelectItem>
                  )}
                </SelectContent>
              </Select>
              {errors.orderId && (
                <p className="text-sm text-red-600 mt-1">{errors.orderId.message}</p>
              )}
            </div>

            <div>
              <Label htmlFor="title">Title *</Label>
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
                onCheckedChange={(checked) => setValue("watermarkEnabled", checked as boolean)}
              />
              <Label htmlFor="watermark" className="text-sm font-normal cursor-pointer">
                Enable watermark on images
              </Label>
            </div>
          </CardContent>
        </Card>

        {/* Media Upload */}
        <Card>
          <CardHeader>
            <CardTitle>Media Files</CardTitle>
            <CardDescription>Upload photos and videos for this delivery</CardDescription>
          </CardHeader>
          <CardContent>
            <DeliveryMediaUploader
              ownerStudioId={user.studioId}
              onUploadComplete={(assets) => {
                setUploadedMedia((prev) => [...prev, ...assets]);
              }}
              maxFiles={100}
              maxSizeMB={500}
            />
          </CardContent>
        </Card>

        {/* Actions */}
        <div className="flex justify-end gap-4">
          <Link href="/dashboard/deliveries">
            <Button type="button" variant="outline" disabled={isSaving}>
              Cancel
            </Button>
          </Link>
          <Button
            type="submit"
            variant="outline"
            disabled={isSaving || uploadedMedia.length === 0}
          >
            {isSaving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <Save className="mr-2 h-4 w-4" />
            Save Draft
          </Button>
          <Button
            type="button"
            onClick={handleSubmit((data) => handleSave(data, true))}
            disabled={isSaving || uploadedMedia.length === 0}
          >
            {isSaving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <Send className="mr-2 h-4 w-4" />
            Save & Publish
          </Button>
        </div>
      </form>
    </div>
  );
}
