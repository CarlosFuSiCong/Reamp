"use client";

import { format } from "date-fns";
import Link from "next/link";
import { Eye, Package, ChevronDown, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { DeliveryPackageListDto, ShootOrder } from "@/types";
import { LoadingState } from "@/components/shared";
import { DeliveryStatusBadge } from "./delivery-status-badge";
import { useState } from "react";
import { getOrderStatusConfig } from "@/lib/utils/enum-labels";

interface DeliveriesByOrderTableProps {
  ordersWithDeliveries: Array<{
    order: ShootOrder;
    deliveries: DeliveryPackageListDto[];
  }>;
  isLoading: boolean;
  searchQuery: string;
  statusFilter: string;
}

export function DeliveriesByOrderTable({
  ordersWithDeliveries,
  isLoading,
  searchQuery,
  statusFilter,
}: DeliveriesByOrderTableProps) {
  const [expandedOrders, setExpandedOrders] = useState<Set<string>>(new Set());

  if (isLoading) {
    return <LoadingState message="Loading deliveries..." />;
  }

  // Filter orders and deliveries
  const filteredData = ordersWithDeliveries
    .map(({ order, deliveries }) => ({
      order,
      deliveries: deliveries.filter((delivery) => {
        const matchesSearch = delivery.title
          .toLowerCase()
          .includes(searchQuery.toLowerCase());
        const matchesStatus =
          statusFilter === "all" || delivery.status.toString() === statusFilter;
        return matchesSearch && matchesStatus;
      }),
    }))
    .filter(({ deliveries }) => deliveries.length > 0);

  if (filteredData.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center h-64 border-2 border-dashed rounded-lg">
        <Package className="h-12 w-12 text-muted-foreground mb-4" />
        <p className="text-muted-foreground">
          {searchQuery || statusFilter !== "all"
            ? "No deliveries found matching your filters"
            : "No deliveries yet"}
        </p>
      </div>
    );
  }

  const toggleOrder = (orderId: string) => {
    const newExpanded = new Set(expandedOrders);
    if (newExpanded.has(orderId)) {
      newExpanded.delete(orderId);
    } else {
      newExpanded.add(orderId);
    }
    setExpandedOrders(newExpanded);
  };

  return (
    <div className="space-y-4">
      {filteredData.map(({ order, deliveries }) => {
        const isExpanded = expandedOrders.has(order.id);
        const totalItems = deliveries.reduce((sum, d) => sum + d.itemCount, 0);
        const statusConfig = getOrderStatusConfig(order.status);

        return (
          <div key={order.id} className="border rounded-lg overflow-hidden">
            {/* Order Header */}
            <div
              className="bg-muted/50 p-4 cursor-pointer hover:bg-muted/70 transition-colors"
              onClick={() => toggleOrder(order.id)}
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-4">
                  <Button
                    variant="ghost"
                    size="sm"
                    className="h-6 w-6 p-0"
                    onClick={(e) => {
                      e.stopPropagation();
                      toggleOrder(order.id);
                    }}
                  >
                    {isExpanded ? (
                      <ChevronDown className="h-4 w-4" />
                    ) : (
                      <ChevronRight className="h-4 w-4" />
                    )}
                  </Button>
                  <div>
                    <div className="flex items-center gap-2">
                      <h3 className="font-semibold">
                        Order #{order.id.slice(0, 8)}
                      </h3>
                      <Badge variant={statusConfig.variant as any}>
                        {statusConfig.label}
                      </Badge>
                    </div>
                    <p className="text-sm text-muted-foreground mt-1">
                      {order.listingTitle || "Listing"}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-6 text-sm">
                  <div className="text-right">
                    <p className="text-muted-foreground">Deliveries</p>
                    <p className="font-medium">{deliveries.length}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-muted-foreground">Total Files</p>
                    <p className="font-medium">{totalItems}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-muted-foreground">Scheduled</p>
                    <p className="font-medium">
                      {order.scheduledStartUtc
                        ? format(new Date(order.scheduledStartUtc), "MMM d")
                        : "TBD"}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            {/* Deliveries List */}
            {isExpanded && (
              <div className="divide-y">
                {deliveries.map((delivery) => (
                  <div
                    key={delivery.id}
                    className="p-4 hover:bg-muted/30 transition-colors"
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <div className="flex items-center gap-3">
                          <p className="font-medium">{delivery.title}</p>
                          <DeliveryStatusBadge status={delivery.status} className="text-xs" />
                        </div>
                        <div className="flex items-center gap-4 mt-2 text-sm text-muted-foreground">
                          <span>{delivery.itemCount} files</span>
                          <span>•</span>
                          <span>
                            Created {format(new Date(delivery.createdAtUtc), "MMM d, yyyy 'at' HH:mm")}
                          </span>
                        </div>
                      </div>
                      <Link href={`/dashboard/deliveries/${delivery.id}`}>
                        <Button variant="ghost" size="sm">
                          <Eye className="h-4 w-4 mr-2" />
                          View
                        </Button>
                      </Link>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}
