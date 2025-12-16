"use client";

import { useState } from "react";
import Link from "next/link";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useOrders } from "@/lib/hooks/use-orders";
import { useProfile } from "@/lib/hooks";
import { OrderStatus } from "@/types";
import { getOrderStatusConfig } from "@/lib/utils/enum-labels";
import { Users, Calendar, MapPin, Building2, ChevronRight } from "lucide-react";
import { formatDistanceToNow } from "date-fns";

export default function StaffAssignmentPage() {
  const { user: profile, isLoading: profileLoading } = useProfile();
  const [selectedOrders, setSelectedOrders] = useState<Set<string>>(new Set());

  const isStudio = !!profile?.studioRole;
  const studioId = profile?.studioId;
  
  // Check if user has permission to assign staff (Owner = 3, Manager = 2)
  const roleValue = profile?.studioRole
    ? typeof profile.studioRole === "string"
      ? parseInt(profile.studioRole, 10)
      : Number(profile.studioRole)
    : 0;
  const canAssignStaff = roleValue === 2 || roleValue === 3;

  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders({
    studioId: isStudio ? studioId : undefined,
    status: OrderStatus.Accepted, // Only show Accepted orders
  });

  if (profileLoading || ordersLoading) {
    return <LoadingState message="Loading orders..." />;
  }

  if (ordersError) {
    return <ErrorState message="Failed to load orders" />;
  }

  if (!isStudio || !studioId) {
    return <ErrorState message="Access denied. Studio membership required." />;
  }

  if (!canAssignStaff) {
    return (
      <div className="space-y-6">
        <PageHeader
          title="Staff Assignment"
          description="Permission required"
        />
        <Card className="p-12">
          <div className="text-center">
            <Users className="mx-auto h-12 w-12 text-muted-foreground" />
            <h3 className="mt-4 text-lg font-semibold">Access Denied</h3>
            <p className="mt-2 text-sm text-muted-foreground">
              Only Studio Owners and Managers can assign staff to orders.
            </p>
            <Button asChild className="mt-4" variant="outline">
              <Link href="/dashboard/orders">
                View My Orders
              </Link>
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  const orders = ordersData?.items || [];
  const statusConfig = getOrderStatusConfig(OrderStatus.Accepted);

  const toggleOrderSelection = (orderId: string) => {
    const newSelection = new Set(selectedOrders);
    if (newSelection.has(orderId)) {
      newSelection.delete(orderId);
    } else {
      newSelection.add(orderId);
    }
    setSelectedOrders(newSelection);
  };

  const toggleSelectAll = () => {
    if (selectedOrders.size === orders.length) {
      setSelectedOrders(new Set());
    } else {
      setSelectedOrders(new Set(orders.map((o) => o.id)));
    }
  };

  return (
    <div className="space-y-6">
      <PageHeader
        title="Staff Assignment"
        description="Manage staff assignments for accepted orders"
      />

      <Card className="p-6">
        <div className="space-y-4">
          {/* Summary Stats */}
          <div className="flex items-center justify-between pb-4 border-b">
            <div className="flex items-center gap-4">
              <div>
                <p className="text-sm text-muted-foreground">Total Orders</p>
                <p className="text-2xl font-bold">{orders.length}</p>
              </div>
              <div className="h-12 w-px bg-border" />
              <div>
                <p className="text-sm text-muted-foreground">Status</p>
                <Badge variant={statusConfig.variant as any}>
                  {statusConfig.label}
                </Badge>
              </div>
              {selectedOrders.size > 0 && (
                <>
                  <div className="h-12 w-px bg-border" />
                  <div>
                    <p className="text-sm text-muted-foreground">Selected</p>
                    <p className="text-lg font-semibold">{selectedOrders.size}</p>
                  </div>
                </>
              )}
            </div>

            {selectedOrders.size > 0 && (
              <Button disabled>
                <Users className="mr-2 h-4 w-4" />
                Batch Assign ({selectedOrders.size})
              </Button>
            )}
          </div>

          {/* Orders Table */}
          {orders.length === 0 ? (
            <div className="text-center py-12">
              <Users className="mx-auto h-12 w-12 text-muted-foreground" />
              <h3 className="mt-4 text-lg font-semibold">No orders to assign</h3>
              <p className="mt-2 text-sm text-muted-foreground">
                All accepted orders have been assigned to staff members.
              </p>
              <Button asChild className="mt-4" variant="outline">
                <Link href="/dashboard/marketplace">
                  Browse Marketplace
                </Link>
              </Button>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-12">
                    <input
                      type="checkbox"
                      checked={selectedOrders.size === orders.length && orders.length > 0}
                      onChange={toggleSelectAll}
                      className="rounded border-gray-300"
                    />
                  </TableHead>
                  <TableHead>Order</TableHead>
                  <TableHead>Listing</TableHead>
                  <TableHead>Agency</TableHead>
                  <TableHead>Amount</TableHead>
                  <TableHead>Created</TableHead>
                  <TableHead className="text-right">Action</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {orders.map((order) => (
                  <TableRow key={order.id}>
                    <TableCell>
                      <input
                        type="checkbox"
                        checked={selectedOrders.has(order.id)}
                        onChange={() => toggleOrderSelection(order.id)}
                        className="rounded border-gray-300"
                      />
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-col gap-1">
                        <span className="font-medium">
                          #{order.id.substring(0, 8)}
                        </span>
                        <Badge variant="outline" className="w-fit">
                          {statusConfig.label}
                        </Badge>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-col gap-1">
                        <span className="font-medium">
                          {order.listingTitle || "Untitled"}
                        </span>
                        {order.listingAddress && (
                          <span className="text-sm text-muted-foreground flex items-center gap-1">
                            <MapPin className="h-3 w-3" />
                            {order.listingAddress}
                          </span>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      {order.agencyName ? (
                        <div className="flex items-center gap-2">
                          <Building2 className="h-4 w-4 text-muted-foreground" />
                          <span className="text-sm">{order.agencyName}</span>
                        </div>
                      ) : (
                        <span className="text-sm text-muted-foreground">N/A</span>
                      )}
                    </TableCell>
                    <TableCell>
                      <div className="flex flex-col gap-1">
                        <span className="font-semibold">
                          {order.currency} ${order.totalAmount.toFixed(2)}
                        </span>
                        {order.taskCount !== undefined && order.taskCount > 0 && (
                          <span className="text-xs text-muted-foreground">
                            {order.taskCount} task(s)
                          </span>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <span className="text-sm text-muted-foreground">
                        {formatDistanceToNow(new Date(order.createdAtUtc), {
                          addSuffix: true,
                        })}
                      </span>
                    </TableCell>
                    <TableCell className="text-right">
                      <Button asChild size="sm" variant="outline">
                        <Link href={`/dashboard/orders/${order.id}/assign`}>
                          <Users className="mr-2 h-4 w-4" />
                          Assign
                        </Link>
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </div>
      </Card>
    </div>
  );
}
