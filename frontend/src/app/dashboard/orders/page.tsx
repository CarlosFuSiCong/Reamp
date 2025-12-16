"use client";

import { useState } from "react";
import Link from "next/link";
import {
  PageHeader,
  LoadingState,
  ErrorState,
  Pagination,
} from "@/components/shared";
import { OrdersFilters } from "@/components/orders";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
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
  XCircle
} from "lucide-react";
import { formatDistanceToNow } from "date-fns";

export default function OrdersPage() {
  const { user: profile, isLoading: profileLoading } = useProfile();
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "all">("all");
  const [searchKeyword, setSearchKeyword] = useState("");
  const [page, setPage] = useState(1);

  // Check user role
  const isAgent = profile?.agencyRole !== undefined && profile?.agencyRole !== null;
  const isStudio = profile?.studioRole !== undefined && profile?.studioRole !== null;

  // Query orders based on user role
  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders({
    agencyId: isAgent ? profile?.agencyId : undefined,
    studioId: isStudio ? profile?.studioId : undefined,
    status: statusFilter === "all" ? undefined : statusFilter,
    keyword: searchKeyword || undefined,
    page,
    pageSize: 20,
  });

  const handleStatusChange = (newStatus: OrderStatus | "all") => {
    setStatusFilter(newStatus);
    setPage(1);
  };

  const handleSearchChange = (newKeyword: string) => {
    setSearchKeyword(newKeyword);
    setPage(1);
  };

  // Get status icon
  const getStatusIcon = (status: OrderStatus) => {
    switch (status) {
      case OrderStatus.Placed:
        return <Clock className="h-4 w-4" />;
      case OrderStatus.Accepted:
        return <CheckCircle className="h-4 w-4" />;
      case OrderStatus.Scheduled:
        return <Calendar className="h-4 w-4" />;
      case OrderStatus.InProgress:
        return <Camera className="h-4 w-4" />;
      case OrderStatus.AwaitingConfirmation:
        return <Package className="h-4 w-4" />;
      case OrderStatus.Completed:
        return <CheckCircle className="h-4 w-4" />;
      case OrderStatus.Cancelled:
        return <XCircle className="h-4 w-4" />;
      default:
        return <Clock className="h-4 w-4" />;
    }
  };

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
                  <Badge variant={statusConfig.variant as any} className="flex items-center gap-1">
                    {getStatusIcon(order.status)}
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
                <p className="text-xs text-muted-foreground">Total Amount</p>
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
              <div>
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

  if (profileLoading) {
    return <LoadingState message="Loading profile..." />;
  }

  if (ordersLoading) {
    return <LoadingState message="Loading orders..." />;
  }

  if (ordersError) {
    return <ErrorState message="Failed to load orders" />;
  }

  // Page title and description based on role
  const pageTitle = isStudio ? "My Orders" : "Orders";
  const pageDescription = isStudio
    ? "Manage orders you've accepted from the marketplace"
    : "Manage your photography orders";

  const orders = ordersData?.items || [];

  return (
    <div className="space-y-6">
      <PageHeader
        title={pageTitle}
        description={pageDescription}
        action={
          isAgent ? (
            <Button asChild>
              <Link href="/dashboard/orders/new">
                <Plus className="mr-2 h-4 w-4" />
                New Order
              </Link>
            </Button>
          ) : undefined
        }
      />

      {/* Filters */}
      <OrdersFilters
        statusFilter={statusFilter}
        onStatusChange={handleStatusChange}
        searchKeyword={searchKeyword}
        onSearchChange={handleSearchChange}
      />

      {/* Orders Grid */}
      {orders.length === 0 ? (
        <Card className="p-12">
          <div className="text-center">
            <Package className="mx-auto h-12 w-12 text-muted-foreground" />
            <h3 className="mt-4 text-lg font-semibold">No orders found</h3>
            <p className="mt-2 text-sm text-muted-foreground">
              {statusFilter === "all"
                ? "You don't have any orders yet."
                : "No orders match the selected filters."}
            </p>
            {isAgent && statusFilter === "all" && (
              <Button asChild className="mt-4">
                <Link href="/dashboard/orders/new">
                  <Plus className="mr-2 h-4 w-4" />
                  Create New Order
                </Link>
              </Button>
            )}
          </div>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {orders.map(renderOrderCard)}
        </div>
      )}

      {/* Pagination */}
      {ordersData && ordersData.total > 20 && (
        <Pagination
          currentPage={page}
          totalItems={ordersData.total}
          pageSize={20}
          onPageChange={setPage}
        />
      )}
    </div>
  );
}
