"use client";

import { use } from "react";
import Link from "next/link";
import { format } from "date-fns";
import { ArrowLeft, XCircle, Camera, Home } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import {
  OrderStatusTimeline,
  OrderTasksCard,
  OrderSummaryCard,
  OrderInfoCard,
  OrderActions,
} from "@/components/orders";
import { Button } from "@/components/ui/button";
import { useOrder } from "@/lib/hooks/use-orders";
import { useListing } from "@/lib/hooks/use-listings";
import { useStudio } from "@/lib/hooks/use-studios";
import { useProfile } from "@/lib/hooks";
import { OrderStatus } from "@/types";

export default function OrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  const orderId = resolvedParams.id;

  const { data: order, isLoading, error } = useOrder(orderId);
  const { data: listing } = useListing(order?.listingId || null);

  // Filter out empty GUID - marketplace orders have null or empty studio
  const validStudioId =
    order?.studioId && order.studioId !== "00000000-0000-0000-0000-000000000000"
      ? order.studioId
      : null;
  const { data: studio } = useStudio(validStudioId);

  const { user: profile } = useProfile();
  const isAgent = !!profile?.agencyRole;
  const isStudio = !!profile?.studioRole;

  if (isLoading) {
    return <LoadingState message="Loading order details..." />;
  }

  if (error || !order) {
    return <ErrorState message="Failed to load order details" />;
  }

  const tasksCompleted = order.tasks?.filter((t) => t.status === 4).length || 0;
  const isCancelled = order.status === OrderStatus.Cancelled;

  return (
    <div className="space-y-6">
      <PageHeader
        title={order.title || `Order #${order.id.substring(0, 8)}`}
        description={`Created ${format(new Date(order.createdAtUtc), "PPP")}`}
        action={
          <Button variant="outline" asChild>
            <Link href="/dashboard/orders">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to Orders
            </Link>
          </Button>
        }
      />

      <OrderActions
        orderId={order.id}
        currentStatus={order.status}
        isAgent={isAgent}
        isStudio={isStudio}
      />

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          <OrderStatusTimeline currentStatus={order.status} isCancelled={isCancelled} />

          {isCancelled && order.cancellationReason && (
            <OrderInfoCard title="Cancellation Reason" icon={<XCircle className="h-5 w-5" />}>
              <p className="text-sm text-muted-foreground">{order.cancellationReason}</p>
            </OrderInfoCard>
          )}

          <OrderTasksCard tasks={order.tasks || []} currency={order.currency} />
        </div>

        <div className="space-y-6">
          <OrderSummaryCard
            totalAmount={order.totalAmount}
            currency={order.currency}
            tasksCompleted={tasksCompleted}
            totalTasks={order.tasks?.length || 0}
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

          {(order.status === OrderStatus.Completed ||
            order.status === OrderStatus.AwaitingConfirmation) && (
            <Button className="w-full" asChild>
              <Link href={`/dashboard/deliveries?orderId=${order.id}`}>View Deliveries</Link>
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
