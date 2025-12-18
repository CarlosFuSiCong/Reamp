"use client";

import Link from "next/link";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { OrderCard, OrdersEmptyState, StudioWorkflowTabs } from "@/components/orders";
import { useOrders } from "@/lib/hooks/use-orders";
import { useProfile } from "@/lib/hooks";
import { useUserRole } from "@/lib/hooks/use-user-role";
import { useOrdersWorkflow } from "@/lib/hooks/use-orders-workflow";
import { Plus, Package } from "lucide-react";

export default function OrdersPage() {
  const { user: profile, isLoading: profileLoading } = useProfile();
  const { isAgent, isStudio } = useUserRole(profile);

  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders({
    agencyId: isAgent ? profile?.agencyId : undefined,
    studioId: isStudio ? profile?.studioId : undefined,
  });

  const orders = ordersData?.items || [];
  const workflowGroups = useOrdersWorkflow(orders);

  if (profileLoading || ordersLoading) {
    return <LoadingState message="Loading orders..." />;
  }

  if (ordersError) {
    return <ErrorState message="Failed to load orders" />;
  }

  // Studio view: Workflow tabs
  if (isStudio) {
    return (
      <div className="space-y-6">
        <PageHeader
          title="My Orders"
          description="Manage your accepted orders through each workflow step"
        />
        <StudioWorkflowTabs {...workflowGroups} />
      </div>
    );
  }

  // Agent view: Grid of all orders
  return (
    <div className="space-y-6">
      <PageHeader
        title="Orders"
        description="Manage your photography orders"
        action={
          <Button asChild>
            <Link href="/dashboard/orders/new">
              <Plus className="mr-2 h-4 w-4" />
              New Order
            </Link>
          </Button>
        }
      />

      {orders.length === 0 ? (
        <OrdersEmptyState
          icon={Package}
          title="No orders yet"
          description="Create your first order to get started."
          action={
            <Button asChild>
              <Link href="/dashboard/orders/new">
                <Plus className="mr-2 h-4 w-4" />
                Create New Order
              </Link>
            </Button>
          }
        />
      ) : (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {orders.map((order) => (
            <OrderCard key={order.id} order={order} isAgent />
          ))}
        </div>
      )}
    </div>
  );
}
