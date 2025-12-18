"use client";

import Link from "next/link";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { OrderCard, OrdersEmptyState, StudioWorkflowTabs } from "@/components/orders";
import { useOrders } from "@/lib/hooks/use-orders";
import { useProfile } from "@/lib/hooks";
import { useUserRole } from "@/lib/hooks/use-user-role";
import { useOrdersWorkflow } from "@/lib/hooks/use-orders-workflow";
import { Plus, Package, ShoppingCart } from "lucide-react";

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
        {/* Header */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-purple-600 to-pink-700 flex items-center justify-center text-white">
                <ShoppingCart className="h-5 w-5" />
              </div>
              <div>
                <h1 className="text-3xl font-bold tracking-tight text-gray-900">My Orders</h1>
                <p className="text-muted-foreground mt-0.5">
                  Manage your accepted orders through each workflow step
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Workflow Tabs */}
        <Card className="border-0 shadow-lg">
          <CardContent className="p-6">
            <StudioWorkflowTabs {...workflowGroups} />
          </CardContent>
        </Card>
      </div>
    );
  }

  // Agent view: Grid of all orders
  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-blue-600 to-indigo-700 flex items-center justify-center text-white">
              <ShoppingCart className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">Orders</h1>
              <p className="text-muted-foreground mt-0.5">
                Manage your photography orders
              </p>
            </div>
          </div>
        </div>
        <Button asChild className="shadow-lg hover:shadow-xl transition-all">
          <Link href="/dashboard/orders/new">
            <Plus className="mr-2 h-4 w-4" />
            New Order
          </Link>
        </Button>
      </div>

      {/* Orders Grid */}
      {orders.length === 0 ? (
        <Card className="border-0 shadow-lg">
          <CardContent className="p-0">
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-24 w-24 rounded-full bg-gradient-to-br from-purple-100 to-pink-100 flex items-center justify-center mb-6">
                <Package className="h-12 w-12 text-purple-600" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">No orders yet</h3>
              <p className="text-gray-600 mb-6 max-w-md mx-auto">
                Create your first order to get started with professional photography services
              </p>
              <Button asChild size="lg" className="shadow-lg">
                <Link href="/dashboard/orders/new">
                  <Plus className="mr-2 h-5 w-5" />
                  Create Your First Order
                </Link>
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {orders.map((order) => (
            <OrderCard key={order.id} order={order} isAgent />
          ))}
        </div>
      )}
    </div>
  );
}
