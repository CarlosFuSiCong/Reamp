"use client";

import { use, useState } from "react";
import { useRouter } from "next/navigation";
import {
  PageHeader,
  LoadingState,
  ErrorState,
  ConfirmDialog,
} from "@/components/shared";
import {
  OrderStatusTimeline,
  OrderTasksCard,
  OrderSummaryCard,
  OrderInfoCard,
} from "@/components/orders";
import { Button } from "@/components/ui/button";
import { useOrder, useCancelOrder } from "@/lib/hooks/use-orders";
import { useListing } from "@/lib/hooks/use-listings";
import { useStudio } from "@/lib/hooks/use-studios";
import { OrderStatus } from "@/types";
import { ArrowLeft, XCircle, Building2, Camera, Home } from "lucide-react";
import Link from "next/link";
import { format } from "date-fns";

export default function OrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  const router = useRouter();
  const orderId = resolvedParams.id;
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);

  const { data: order, isLoading, error } = useOrder(orderId);
  const { data: listing } = useListing(order?.listingId || null);
  const { data: studio } = useStudio(order?.studioId || null);
  const cancelMutation = useCancelOrder();

  const handleCancel = () => {
    cancelMutation.mutate({ id: orderId });
    setCancelDialogOpen(false);
  };

  const canCancelOrder =
    order?.status === OrderStatus.Placed ||
    order?.status === OrderStatus.Accepted;

  if (isLoading) {
    return <LoadingState />;
  }

  if (error || !order) {
    return <ErrorState message="Failed to load order details" />;
  }

  const tasksCompleted = order.tasks.filter((t) => t.status === 4).length;
  const isCancelled = order.status === OrderStatus.Cancelled;

  return (
    <div className="space-y-6">
      <PageHeader
        title={`Order #${order.id.substring(0, 8)}`}
        description={`Created ${format(new Date(order.createdAtUtc), "PPP")}`}
        action={
          <div className="flex gap-2">
            <Button variant="outline" asChild>
              <Link href="/dashboard/agency/orders">
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to Orders
              </Link>
            </Button>
            {canCancelOrder && (
              <Button
                variant="destructive"
                onClick={() => setCancelDialogOpen(true)}
              >
                <XCircle className="mr-2 h-4 w-4" />
                Cancel Order
              </Button>
            )}
          </div>
        }
      />

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          <OrderStatusTimeline
            currentStatus={order.status}
            isCancelled={isCancelled}
          />

          {isCancelled && order.cancellationReason && (
            <OrderInfoCard title="Cancellation Reason" icon={<XCircle className="h-5 w-5" />}>
              <p className="text-sm text-muted-foreground">
                {order.cancellationReason}
              </p>
            </OrderInfoCard>
          )}

          <OrderTasksCard tasks={order.tasks} currency={order.currency} />
        </div>

        <div className="space-y-6">
          <OrderSummaryCard
            totalAmount={order.totalAmount}
            currency={order.currency}
            tasksCompleted={tasksCompleted}
            totalTasks={order.tasks.length}
          />

          {listing && (
            <OrderInfoCard title="Listing" icon={<Home className="h-5 w-5" />}>
              <div className="space-y-2">
                <p className="font-medium">{listing.title}</p>
                <p className="text-sm text-muted-foreground">
                  {listing.addressLine1}
                  {listing.addressLine2 && `, ${listing.addressLine2}`}
                </p>
                <p className="text-sm text-muted-foreground">
                  {listing.city}, {listing.state} {listing.postcode}
                </p>
                <Button variant="outline" size="sm" asChild className="mt-2">
                  <Link href={`/dashboard/agency/listings/${listing.id}`}>
                    View Listing
                  </Link>
                </Button>
              </div>
            </OrderInfoCard>
          )}

          {studio && (
            <OrderInfoCard title="Studio" icon={<Camera className="h-5 w-5" />}>
              <div className="space-y-2">
                <p className="font-medium">{studio.name}</p>
                <p className="text-sm text-muted-foreground">{studio.email}</p>
                {studio.phone && (
                  <p className="text-sm text-muted-foreground">{studio.phone}</p>
                )}
              </div>
            </OrderInfoCard>
          )}

          {order.status === OrderStatus.Completed && (
            <Button className="w-full" asChild>
              <Link href={`/dashboard/agency/orders/${order.id}/delivery`}>
                View Delivery
              </Link>
            </Button>
          )}
        </div>
      </div>

      <ConfirmDialog
        open={cancelDialogOpen}
        onOpenChange={setCancelDialogOpen}
        title="Cancel Order"
        description="Are you sure you want to cancel this order? This action cannot be undone."
        onConfirm={handleCancel}
        confirmText="Cancel Order"
        variant="destructive"
      />
    </div>
  );
}
