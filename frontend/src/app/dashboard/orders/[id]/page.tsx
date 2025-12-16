"use client";

import { use, useState } from "react";
import { PageHeader, LoadingState, ErrorState, ConfirmDialog } from "@/components/shared";
import {
  OrderStatusTimeline,
  OrderTasksCard,
  OrderSummaryCard,
  OrderInfoCard,
} from "@/components/orders";
import { Button } from "@/components/ui/button";
import {
  useOrder,
  useCancelOrder,
  useAcceptOrder,
  useStartOrder,
  useCompleteOrder,
} from "@/lib/hooks/use-orders";
import { useListing } from "@/lib/hooks/use-listings";
import { useStudio } from "@/lib/hooks/use-studios";
import { useProfile } from "@/lib/hooks";
import { OrderStatus } from "@/types";
import { ArrowLeft, XCircle, Camera, Home } from "lucide-react";
import Link from "next/link";
import { format } from "date-fns";

export default function OrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  const orderId = resolvedParams.id;
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [confirmAction, setConfirmAction] = useState<{
    open: boolean;
    action: "accept" | "start" | "complete" | null;
    title: string;
    description: string;
  }>({
    open: false,
    action: null,
    title: "",
    description: "",
  });

  const { data: order, isLoading, error } = useOrder(orderId);
  const { data: listing } = useListing(order?.listingId || null);
  // Filter out empty GUID for studio - marketplace orders have null or empty GUID
  const validStudioId =
    order?.studioId && order.studioId !== "00000000-0000-0000-0000-000000000000"
      ? order.studioId
      : null;
  const { data: studio } = useStudio(validStudioId);
  const { user: profile } = useProfile();
  const cancelMutation = useCancelOrder();
  const acceptMutation = useAcceptOrder();
  const startMutation = useStartOrder();
  const completeMutation = useCompleteOrder();

  const isStaff = profile?.studioRole !== undefined && profile?.studioRole !== null;

  const handleCancel = () => {
    cancelMutation.mutate({ id: orderId });
    setCancelDialogOpen(false);
  };

  const handleAction = (action: "accept" | "start" | "complete") => {
    const configs = {
      accept: {
        title: "Accept Order",
        description:
          "Are you sure you want to accept this order? This will assign the order to you.",
      },
      start: {
        title: "Start Shoot",
        description:
          "Are you sure you want to start this shoot? This will mark the order as in progress.",
      },
      complete: {
        title: "Complete Order",
        description: "Are you sure you want to mark this order as completed?",
      },
    };

    setConfirmAction({
      open: true,
      action,
      ...configs[action],
    });
  };

  const confirmActionHandler = async () => {
    if (!confirmAction.action) return;

    try {
      switch (confirmAction.action) {
        case "accept":
          await acceptMutation.mutateAsync(orderId);
          break;
        case "start":
          await startMutation.mutateAsync(orderId);
          break;
        case "complete":
          await completeMutation.mutateAsync(orderId);
          break;
      }
      setConfirmAction({ open: false, action: null, title: "", description: "" });
    } catch {
      // Error is handled by the mutation
    }
  };

  // Only agents can cancel orders
  const canCancelOrder =
    !isStaff && (order?.status === OrderStatus.Placed || order?.status === OrderStatus.Accepted);

  const canAccept =
    isStaff &&
    (order?.status === OrderStatus.Placed || order?.status === OrderStatus.Accepted) &&
    !order?.assignedPhotographerId;
  const canStart = isStaff && order?.status === OrderStatus.Scheduled;
  const canComplete = isStaff && order?.status === OrderStatus.InProgress;

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
        title={order.title || `Order #${order.id.substring(0, 8)}`}
        description={`Created ${format(new Date(order.createdAtUtc), "PPP")}`}
        action={
          <div className="flex gap-2">
            <Button variant="outline" asChild>
              <Link href="/dashboard/orders">
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to Orders
              </Link>
            </Button>
            {canCancelOrder && (
              <Button variant="destructive" onClick={() => setCancelDialogOpen(true)}>
                <XCircle className="mr-2 h-4 w-4" />
                Cancel Order
              </Button>
            )}
          </div>
        }
      />

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          {/* Only show timeline for agents, not staff */}
          {!isStaff && (
            <OrderStatusTimeline currentStatus={order.status} isCancelled={isCancelled} />
          )}

          {isCancelled && order.cancellationReason && (
            <OrderInfoCard title="Cancellation Reason" icon={<XCircle className="h-5 w-5" />}>
              <p className="text-sm text-muted-foreground">{order.cancellationReason}</p>
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
                  <Link href={`/dashboard/listings/${listing.id}`}>View Listing</Link>
                </Button>
              </div>
            </OrderInfoCard>
          )}

          {studio && (
            <OrderInfoCard title="Studio" icon={<Camera className="h-5 w-5" />}>
              <div className="space-y-2">
                <p className="font-medium">{studio.name}</p>
                <p className="text-sm text-muted-foreground">{studio.email}</p>
                {studio.phone && <p className="text-sm text-muted-foreground">{studio.phone}</p>}
              </div>
            </OrderInfoCard>
          )}

          {order.status === OrderStatus.Completed && (
            <Button className="w-full" asChild>
              <Link href={`/dashboard/orders/${order.id}/delivery`}>View Delivery</Link>
            </Button>
          )}

          {isStaff && (
            <>
              {canAccept && (
                <Button
                  className="w-full"
                  onClick={() => handleAction("accept")}
                  disabled={acceptMutation.isPending}
                >
                  Accept Order
                </Button>
              )}

              {canStart && (
                <Button
                  className="w-full"
                  onClick={() => handleAction("start")}
                  disabled={startMutation.isPending}
                >
                  Start Shoot
                </Button>
              )}

              {canComplete && (
                <Button
                  className="w-full"
                  onClick={() => handleAction("complete")}
                  disabled={completeMutation.isPending}
                >
                  Complete Order
                </Button>
              )}
            </>
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

      <ConfirmDialog
        open={confirmAction.open}
        onOpenChange={(open) => setConfirmAction({ ...confirmAction, open })}
        title={confirmAction.title}
        description={confirmAction.description}
        onConfirm={confirmActionHandler}
        confirmText="Confirm"
      />
    </div>
  );
}
