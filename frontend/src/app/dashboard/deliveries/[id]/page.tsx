"use client";

import { use, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { format } from "date-fns";
import { ArrowLeft, Download, Image as ImageIcon, Send, Loader2 } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DeliveryStatusBadge } from "@/components/deliveries/delivery-status-badge";
import { deliveriesApi } from "@/lib/api";
import { useProfile } from "@/lib/hooks";
import { DeliveryStatus } from "@/types";
import { toast } from "sonner";

export default function DeliveryDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  const deliveryId = resolvedParams.id;
  const { user } = useProfile();

  const {
    data: delivery,
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ["delivery", deliveryId],
    queryFn: () => deliveriesApi.getById(deliveryId),
  });

  const [isPublishing, setIsPublishing] = useState(false);
  const [isRevoking, setIsRevoking] = useState(false);

  const handlePublish = async () => {
    if (!delivery) return;
    setIsPublishing(true);
    try {
      await deliveriesApi.publish(delivery.id);
      toast.success("Delivery package published");
      refetch();
    } catch (error: any) {
      toast.error(error?.message || "Failed to publish delivery");
    } finally {
      setIsPublishing(false);
    }
  };

  const handleRevoke = async () => {
    if (!delivery) return;
    setIsRevoking(true);
    try {
      await deliveriesApi.revoke(delivery.id);
      toast.success("Delivery package revoked");
      refetch();
    } catch (error: any) {
      toast.error(error?.message || "Failed to revoke delivery");
    } finally {
      setIsRevoking(false);
    }
  };

  if (isLoading) {
    return <LoadingState message="Loading delivery..." />;
  }

  if (error || !delivery) {
    return <ErrorState message="Failed to load delivery details" />;
  }

  const isStudio = !!user?.studioId;
  const isDraft = delivery.status === DeliveryStatus.Draft;
  const isPublished = delivery.status === DeliveryStatus.Published;

  return (
    <div className="space-y-6">
      <PageHeader
        title={delivery.title}
        description={`Delivery Package • Order #${delivery.orderId.slice(0, 8)}`}
        action={
          <div className="flex items-center gap-2">
            <Link href="/dashboard/deliveries">
              <Button variant="outline">
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back
              </Button>
            </Link>

            {isStudio && isDraft && (
              <>
                <Link href={`/dashboard/deliveries/${delivery.id}/edit`}>
                  <Button variant="outline">Edit</Button>
                </Link>
                <Button onClick={handlePublish} disabled={isPublishing}>
                  {isPublishing && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  <Send className="mr-2 h-4 w-4" />
                  Publish
                </Button>
              </>
            )}

            {isStudio && isPublished && (
              <Button variant="destructive" onClick={handleRevoke} disabled={isRevoking}>
                {isRevoking && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                Revoke
              </Button>
            )}
          </div>
        }
      />

      <div className="grid gap-6 lg:grid-cols-3">
        {/* Left Column - Details */}
        <div className="lg:col-span-2 space-y-6">
          {/* Status Card */}
          <Card>
            <CardHeader>
              <CardTitle>Delivery Status</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Status</span>
                <DeliveryStatusBadge status={delivery.status} />
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Created</span>
                <span className="text-sm text-muted-foreground">
                  {format(new Date(delivery.createdAtUtc), "MMM d, yyyy 'at' h:mm a")}
                </span>
              </div>
              {delivery.expiresAtUtc && (
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium">Expires</span>
                  <span className="text-sm text-muted-foreground">
                    {format(new Date(delivery.expiresAtUtc), "MMM d, yyyy 'at' h:mm a")}
                  </span>
                </div>
              )}
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Watermark</span>
                <span className="text-sm text-muted-foreground">
                  {delivery.watermarkEnabled ? "Enabled" : "Disabled"}
                </span>
              </div>
            </CardContent>
          </Card>

          {/* Media Items */}
          <Card>
            <CardHeader>
              <CardTitle>Media Files ({delivery.items.length})</CardTitle>
            </CardHeader>
            <CardContent>
              {delivery.items.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">
                  <ImageIcon className="mx-auto h-12 w-12 mb-4 opacity-50" />
                  <p>No media files in this delivery</p>
                </div>
              ) : (
                <div className="grid grid-cols-3 gap-4">
                  {delivery.items.map((item) => (
                    <div
                      key={item.id}
                      className="relative aspect-square rounded-lg overflow-hidden border group"
                    >
                      <div className="w-full h-full bg-gray-100 flex items-center justify-center">
                        {item.thumbnailUrl || item.mediaUrl ? (
                          <img
                            src={item.thumbnailUrl || item.mediaUrl || ""}
                            alt="Media"
                            className="w-full h-full object-cover"
                          />
                        ) : (
                          <ImageIcon className="h-8 w-8 text-gray-400" />
                        )}
                      </div>
                      <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                        <Button variant="secondary" size="sm">
                          <Download className="h-4 w-4" />
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Right Column - Actions */}
        <div className="space-y-6">
          {/* Access Information */}
          <Card>
            <CardHeader>
              <CardTitle>Access</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              {delivery.accesses.length === 0 ? (
                <p className="text-sm text-muted-foreground">
                  No access rules configured
                </p>
              ) : (
                delivery.accesses.map((access) => (
                  <div key={access.id} className="border-b pb-3 last:border-0">
                    <p className="text-sm font-medium">
                      {access.recipientEmail || "Public Link"}
                    </p>
                    {access.maxDownloads && (
                      <p className="text-xs text-muted-foreground">
                        {access.downloads}/{access.maxDownloads} downloads
                      </p>
                    )}
                  </div>
                ))
              )}
            </CardContent>
          </Card>

          {/* Order Info */}
          <Card>
            <CardHeader>
              <CardTitle>Related Order</CardTitle>
            </CardHeader>
            <CardContent>
              <Link href={`/dashboard/orders/${delivery.orderId}`}>
                <Button variant="link" className="p-0 h-auto">
                  View Order #{delivery.orderId.slice(0, 8)}
                </Button>
              </Link>
            </CardContent>
          </Card>

          {/* Listing Info */}
          <Card>
            <CardHeader>
              <CardTitle>Related Listing</CardTitle>
            </CardHeader>
            <CardContent>
              <Link href={`/dashboard/listings/${delivery.listingId}/edit`}>
                <Button variant="link" className="p-0 h-auto">
                  View Listing
                </Button>
              </Link>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
