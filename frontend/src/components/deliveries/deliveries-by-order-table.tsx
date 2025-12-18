"use client";

import { format } from "date-fns";
import Link from "next/link";
import { Eye, Package, ChevronDown, ChevronRight, Calendar, FileText, Layers } from "lucide-react";
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
          <div key={order.id} className="border rounded-lg overflow-hidden shadow-sm hover:shadow-md transition-shadow">
            {/* Order Header - Clickable */}
            <div
              className="bg-gray-50 p-4 cursor-pointer hover:bg-gray-100 transition-colors border-b"
              onClick={() => toggleOrder(order.id)}
            >
              <div className="flex items-center justify-between gap-4">
                <div className="flex items-center gap-4 flex-1 min-w-0">
                  {/* Toggle Icon */}
                  <div className="flex-shrink-0">
                    {isExpanded ? (
                      <ChevronDown className="h-5 w-5 text-gray-600" />
                    ) : (
                      <ChevronRight className="h-5 w-5 text-gray-600" />
                    )}
                  </div>

                  {/* Order Info */}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <h3 className="font-semibold text-gray-900">
                        Order #{order.id.slice(0, 8)}
                      </h3>
                      <Badge variant={statusConfig.variant as any} className="text-xs">
                        {statusConfig.label}
                      </Badge>
                    </div>
                    <p className="text-sm text-gray-600 truncate">
                      {order.listingTitle || "Untitled Listing"}
                    </p>
                  </div>
                </div>

                {/* Stats */}
                <div className="flex items-center gap-6">
                  <div className="flex items-center gap-2">
                    <Package className="h-4 w-4 text-orange-600" />
                    <div className="text-right">
                      <p className="text-xs text-gray-500">Deliveries</p>
                      <p className="font-semibold text-gray-900">{deliveries.length}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <FileText className="h-4 w-4 text-blue-600" />
                    <div className="text-right">
                      <p className="text-xs text-gray-500">Files</p>
                      <p className="font-semibold text-gray-900">{totalItems}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <Calendar className="h-4 w-4 text-green-600" />
                    <div className="text-right">
                      <p className="text-xs text-gray-500">Scheduled</p>
                      <p className="font-semibold text-gray-900">
                        {order.scheduledStartUtc
                          ? format(new Date(order.scheduledStartUtc), "MMM d")
                          : "TBD"}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Deliveries List */}
            {isExpanded && (
              <div className="divide-y bg-white">
                {deliveries.map((delivery) => (
                  <Link
                    key={delivery.id}
                    href={`/dashboard/deliveries/${delivery.id}`}
                    className="block p-4 hover:bg-blue-50/30 transition-colors"
                  >
                    <div className="flex items-center justify-between gap-4">
                      <div className="flex items-center gap-3 flex-1 min-w-0">
                        {/* Delivery Icon */}
                        <div className="h-10 w-10 rounded-lg bg-orange-100 flex items-center justify-center flex-shrink-0">
                          <Layers className="h-5 w-5 text-orange-600" />
                        </div>

                        {/* Delivery Info */}
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2 mb-1">
                            <p className="font-semibold text-gray-900 truncate">{delivery.title}</p>
                            <DeliveryStatusBadge status={delivery.status} className="text-xs flex-shrink-0" />
                          </div>
                          <div className="flex items-center gap-3 text-sm text-gray-600">
                            <span className="flex items-center gap-1">
                              <FileText className="h-3.5 w-3.5" />
                              {delivery.itemCount} files
                            </span>
                            <span>•</span>
                            <span className="flex items-center gap-1">
                              <Calendar className="h-3.5 w-3.5" />
                              {format(new Date(delivery.createdAtUtc), "MMM d, yyyy 'at' HH:mm")}
                            </span>
                          </div>
                        </div>
                      </div>

                      {/* Arrow indicator for link */}
                      <ChevronRight className="h-5 w-5 text-gray-400 flex-shrink-0" />
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}
