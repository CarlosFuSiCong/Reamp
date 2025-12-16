"use client";

import { useState } from "react";
import Link from "next/link";
import {
  PageHeader,
  LoadingState,
  ErrorState,
} from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useOrders } from "@/lib/hooks/use-orders";
import { useProfile } from "@/lib/hooks";
import { OrderStatus, ShootOrder } from "@/types";
import { getOrderStatusConfig } from "@/lib/utils/enum-labels";
import { 
  Plus, 
  Calendar, 
  MapPin, 
  Building2, 
  Camera,
  ChevronRight,
  Clock,
  CheckCircle,
  Package,
  Users,
  Upload
} from "lucide-react";
import { formatDistanceToNow } from "date-fns";

export default function OrdersPage() {
  const { user: profile, isLoading: profileLoading } = useProfile();

  // Check user role
  const isAgent = profile?.agencyRole !== undefined && profile?.agencyRole !== null;
  const isStudio = profile?.studioRole !== undefined && profile?.studioRole !== null;

  // Query all orders for the user
  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders({
    agencyId: isAgent ? profile?.agencyId : undefined,
    studioId: isStudio ? profile?.studioId : undefined,
  });

  // Render order card
  const renderOrderCard = (order: ShootOrder) => {
    const statusConfig = getOrderStatusConfig(order.status);
    
    return (
      <Link key={order.id} href={`/dashboard/orders/${order.id}`}>
        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardHeader className="pb-3">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-2">
                  <Badge variant={statusConfig.variant as any}>
                    {statusConfig.label}
                  </Badge>
                  {order.scheduledStartUtc && (
                    <span className="text-xs text-muted-foreground flex items-center gap-1">
                      <Calendar className="h-3 w-3" />
                      {new Date(order.scheduledStartUtc).toLocaleDateString()}
                    </span>
                  )}
                </div>
                <h3 className="font-semibold text-lg">
                  {order.listingTitle || "Untitled Listing"}
                </h3>
                {order.listingAddress && (
                  <p className="text-sm text-muted-foreground flex items-center gap-1 mt-1">
                    <MapPin className="h-3 w-3" />
                    {order.listingAddress}
                  </p>
                )}
              </div>
              <ChevronRight className="h-5 w-5 text-muted-foreground" />
            </div>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 gap-4 text-sm">
              {/* Agency/Studio info */}
              {isStudio && order.agencyName && (
                <div className="flex items-center gap-2">
                  <Building2 className="h-4 w-4 text-muted-foreground" />
                  <div>
                    <p className="text-xs text-muted-foreground">Agency</p>
                    <p className="font-medium">{order.agencyName}</p>
                  </div>
                </div>
              )}
              {isAgent && order.studioName && (
                <div className="flex items-center gap-2">
                  <Camera className="h-4 w-4 text-muted-foreground" />
                  <div>
                    <p className="text-xs text-muted-foreground">Studio</p>
                    <p className="font-medium">{order.studioName}</p>
                  </div>
                </div>
              )}

              {/* Amount */}
              <div>
                <p className="text-xs text-muted-foreground">Amount</p>
                <p className="font-semibold">
                  {order.currency} ${order.totalAmount.toFixed(2)}
                </p>
              </div>

              {/* Task count */}
              {order.taskCount !== undefined && order.taskCount > 0 && (
                <div>
                  <p className="text-xs text-muted-foreground">Tasks</p>
                  <p className="font-medium">{order.taskCount} task(s)</p>
                </div>
              )}

              {/* Created time */}
              <div className="col-span-2">
                <p className="text-xs text-muted-foreground">Created</p>
                <p className="text-sm">
                  {formatDistanceToNow(new Date(order.createdAtUtc), { addSuffix: true })}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      </Link>
    );
  };

  // Empty state component
  const EmptyState = ({ icon: Icon, title, description }: { icon: any; title: string; description: string }) => (
    <Card className="p-12">
      <div className="text-center">
        <Icon className="mx-auto h-12 w-12 text-muted-foreground" />
        <h3 className="mt-4 text-lg font-semibold">{title}</h3>
        <p className="mt-2 text-sm text-muted-foreground">{description}</p>
      </div>
    </Card>
  );

  if (profileLoading) {
    return <LoadingState message="Loading profile..." />;
  }

  if (ordersLoading) {
    return <LoadingState message="Loading orders..." />;
  }

  if (ordersError) {
    return <ErrorState message="Failed to load orders" />;
  }

  const orders = ordersData?.items || [];

  // For Studio: Group orders by workflow steps
  if (isStudio) {
    // Step 1: Accepted - Need to assign staff (已接单，待分配人员)
    const needAssignment = orders.filter(o => o.status === OrderStatus.Accepted);
    
    // Step 2: Scheduled - Staff assigned, ready to shoot (已分配人员，待开始拍摄)
    const readyToShoot = orders.filter(o => o.status === OrderStatus.Scheduled);
    
    // Step 3: InProgress & AwaitingDelivery - Shooting or waiting for upload (拍摄中/待上传)
    const needUpload = orders.filter(o => 
      o.status === OrderStatus.InProgress || o.status === OrderStatus.AwaitingDelivery
    );
    
    // Step 4: AwaitingConfirmation - Uploaded, waiting for agent confirmation (上传完毕，待Agent确认)
    const awaitingConfirmation = orders.filter(o => o.status === OrderStatus.AwaitingConfirmation);

    return (
      <div className="space-y-6">
        <PageHeader
          title="My Orders"
          description="Manage your accepted orders through each workflow step"
        />

        <Tabs defaultValue="assignment" className="space-y-6">
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="assignment" className="flex items-center gap-2">
              <Users className="h-4 w-4" />
              <span className="hidden sm:inline">Assign Staff</span>
              <span className="sm:hidden">Assign</span>
              {needAssignment.length > 0 && (
                <Badge variant="secondary" className="ml-1">{needAssignment.length}</Badge>
              )}
            </TabsTrigger>
            <TabsTrigger value="shoot" className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              <span className="hidden sm:inline">Ready to Shoot</span>
              <span className="sm:hidden">Shoot</span>
              {readyToShoot.length > 0 && (
                <Badge variant="secondary" className="ml-1">{readyToShoot.length}</Badge>
              )}
            </TabsTrigger>
            <TabsTrigger value="upload" className="flex items-center gap-2">
              <Upload className="h-4 w-4" />
              <span className="hidden sm:inline">Upload Delivery</span>
              <span className="sm:hidden">Upload</span>
              {needUpload.length > 0 && (
                <Badge variant="secondary" className="ml-1">{needUpload.length}</Badge>
              )}
            </TabsTrigger>
            <TabsTrigger value="pending" className="flex items-center gap-2">
              <Clock className="h-4 w-4" />
              <span className="hidden sm:inline">Pending</span>
              <span className="sm:hidden">Pending</span>
              {awaitingConfirmation.length > 0 && (
                <Badge variant="secondary" className="ml-1">{awaitingConfirmation.length}</Badge>
              )}
            </TabsTrigger>
          </TabsList>

          {/* Step 1: Assign Staff */}
          <TabsContent value="assignment" className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold">Assign Staff</h3>
                <p className="text-sm text-muted-foreground">
                  Orders accepted, waiting for staff assignment
                </p>
              </div>
              <Badge variant="outline">{needAssignment.length} orders</Badge>
            </div>
            {needAssignment.length === 0 ? (
              <EmptyState
                icon={Users}
                title="No orders to assign"
                description="All accepted orders have been assigned to staff members."
              />
            ) : (
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {needAssignment.map(renderOrderCard)}
              </div>
            )}
          </TabsContent>

          {/* Step 2: Ready to Shoot */}
          <TabsContent value="shoot" className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold">Ready to Shoot</h3>
                <p className="text-sm text-muted-foreground">
                  Staff assigned, ready to start shooting
                </p>
              </div>
              <Badge variant="outline">{readyToShoot.length} orders</Badge>
            </div>
            {readyToShoot.length === 0 ? (
              <EmptyState
                icon={Calendar}
                title="No scheduled shoots"
                description="No orders are currently scheduled for shooting."
              />
            ) : (
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {readyToShoot.map(renderOrderCard)}
              </div>
            )}
          </TabsContent>

          {/* Step 3: Upload Delivery */}
          <TabsContent value="upload" className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold">Upload Delivery</h3>
                <p className="text-sm text-muted-foreground">
                  Shooting completed, upload delivery package
                </p>
              </div>
              <Badge variant="outline">{needUpload.length} orders</Badge>
            </div>
            {needUpload.length === 0 ? (
              <EmptyState
                icon={Upload}
                title="No uploads pending"
                description="All completed shoots have been uploaded."
              />
            ) : (
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {needUpload.map(renderOrderCard)}
              </div>
            )}
          </TabsContent>

          {/* Step 4: Awaiting Confirmation */}
          <TabsContent value="pending" className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold">Awaiting Confirmation</h3>
                <p className="text-sm text-muted-foreground">
                  Delivery uploaded, waiting for agent confirmation
                </p>
              </div>
              <Badge variant="outline">{awaitingConfirmation.length} orders</Badge>
            </div>
            {awaitingConfirmation.length === 0 ? (
              <EmptyState
                icon={Clock}
                title="No pending confirmations"
                description="No deliveries are currently waiting for confirmation."
              />
            ) : (
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {awaitingConfirmation.map(renderOrderCard)}
              </div>
            )}
          </TabsContent>
        </Tabs>
      </div>
    );
  }

  // For Agent: Simple list view with all orders
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
        <Card className="p-12">
          <div className="text-center">
            <Package className="mx-auto h-12 w-12 text-muted-foreground" />
            <h3 className="mt-4 text-lg font-semibold">No orders yet</h3>
            <p className="mt-2 text-sm text-muted-foreground">
              Create your first order to get started.
            </p>
            <Button asChild className="mt-4">
              <Link href="/dashboard/orders/new">
                <Plus className="mr-2 h-4 w-4" />
                Create New Order
              </Link>
            </Button>
          </div>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {orders.map(renderOrderCard)}
        </div>
      )}
    </div>
  );
}
