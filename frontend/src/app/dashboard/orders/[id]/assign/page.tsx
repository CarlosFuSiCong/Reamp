"use client";

import { use } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { AssignStaffForm } from "@/components/orders/assign-staff-form";
import { useOrder } from "@/lib/hooks/use-orders";
import { useListing } from "@/lib/hooks/use-listings";
import { useProfile } from "@/lib/hooks";
import { getOrderStatusConfig } from "@/lib/utils/enum-labels";
import { OrderStatus } from "@/types";

export default function AssignStaffPage({ params }: { params: Promise<{ id: string }> }) {
  const router = useRouter();
  const resolvedParams = use(params);
  const orderId = resolvedParams.id;

  const { user: profile } = useProfile();
  const { data: order, isLoading: orderLoading, error: orderError } = useOrder(orderId);
  const { data: listing } = useListing(order?.listingId || null);

  const studioId = profile?.studioId;
  const isStudio = !!profile?.studioRole;

  if (orderLoading) {
    return <LoadingState message="Loading order..." />;
  }

  if (orderError || !order) {
    return <ErrorState message="Failed to load order" />;
  }

  if (!isStudio || !studioId) {
    return <ErrorState message="Access denied. Studio membership required." />;
  }

  // Check if order is in correct status for assignment
  if (order.status !== OrderStatus.Accepted) {
    const statusConfig = getOrderStatusConfig(order.status);
    return (
      <div className="space-y-6">
        <PageHeader
          title="Assign Staff"
          description="This order cannot be assigned at this time"
          action={
            <Button variant="outline" asChild>
              <Link href={`/dashboard/orders/${orderId}`}>
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to Order
              </Link>
            </Button>
          }
        />
        <Card>
          <CardHeader>
            <CardTitle>Invalid Order Status</CardTitle>
            <CardDescription>
              Staff can only be assigned to orders in "Accepted" status
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div>
                <p className="text-sm text-muted-foreground mb-2">Current Status:</p>
                <Badge variant={statusConfig.variant as any}>{statusConfig.label}</Badge>
              </div>
              <p className="text-sm text-muted-foreground">
                This order is currently in <strong>{statusConfig.label}</strong> status.
                You can only assign staff to orders that have been accepted by your studio.
              </p>
              <Button asChild>
                <Link href={`/dashboard/orders/${orderId}`}>
                  View Order Details
                </Link>
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  const handleSuccess = () => {
    router.push(`/dashboard/orders/${orderId}`);
  };

  const handleCancel = () => {
    router.back();
  };

  return (
    <div className="space-y-6">
      <PageHeader
        title="Assign Photographer"
        description={`Order #${orderId.substring(0, 8)}`}
        action={
          <Button variant="outline" asChild>
            <Link href={`/dashboard/orders/${orderId}`}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to Order
            </Link>
          </Button>
        }
      />

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <AssignStaffForm
            orderId={orderId}
            studioId={studioId}
            onSuccess={handleSuccess}
            onCancel={handleCancel}
          />
        </div>

        <div className="space-y-6">
          {/* Order Summary */}
          <Card>
            <CardHeader>
              <CardTitle>Order Summary</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <p className="text-sm text-muted-foreground">Status</p>
                <Badge variant={getOrderStatusConfig(order.status).variant as any}>
                  {getOrderStatusConfig(order.status).label}
                </Badge>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Amount</p>
                <p className="text-2xl font-bold">
                  {order.currency} ${order.totalAmount.toFixed(2)}
                </p>
              </div>
              {order.taskCount !== undefined && order.taskCount > 0 && (
                <div>
                  <p className="text-sm text-muted-foreground">Tasks</p>
                  <p className="font-medium">{order.taskCount} task(s)</p>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Listing Info */}
          {listing && (
            <Card>
              <CardHeader>
                <CardTitle>Listing</CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                <p className="font-medium">{listing.title}</p>
                <p className="text-sm text-muted-foreground">
                  {listing.addressLine1}
                  {listing.addressLine2 && `, ${listing.addressLine2}`}
                </p>
                <p className="text-sm text-muted-foreground">
                  {listing.city}, {listing.state} {listing.postcode}
                </p>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  );
}
