"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Card, CardContent } from "@/components/ui/card";
import { OrdersTable, OrdersFilters } from "@/components/orders";
import { useProfile } from "@/lib/hooks";
import { ordersApi } from "@/lib/api";
import { OrderStatus } from "@/types";
import { Store, ShoppingBag } from "lucide-react";

export default function OrdersMarketplacePage() {
  const { user } = useProfile();
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "all">(OrderStatus.Placed);

  const { data, isLoading } = useQuery({
    queryKey: ["orders", "marketplace", statusFilter, searchQuery],
    queryFn: () =>
      ordersApi.list({
        status: statusFilter === "all" ? undefined : statusFilter.toString(),
        page: 1,
        pageSize: 50,
      }),
    enabled: !!user?.studioId,
  });

  if (!user?.studioId) {
    return (
      <div className="space-y-6">
        {/* Header with Icon */}
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-green-600 flex items-center justify-center text-white shadow-md">
              <Store className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">Orders Marketplace</h1>
              <p className="text-sm text-gray-600 mt-1">Browse and accept orders from agents</p>
            </div>
          </div>
        </div>

        {/* Access Restricted Card */}
        <Card className="shadow-lg">
          <CardContent className="p-0">
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-16 w-16 rounded-full bg-red-50 flex items-center justify-center mb-4">
                <Store className="h-8 w-8 text-red-600" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Studio Access Only</h3>
              <p className="text-sm text-gray-600">
                You must be a member of a photography studio to view marketplace orders
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header with Icon */}
      <div>
        <div className="flex items-center gap-3">
          <div className="h-10 w-10 rounded-xl bg-green-600 flex items-center justify-center text-white shadow-md">
            <Store className="h-5 w-5" />
          </div>
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-gray-900">Orders Marketplace</h1>
            <p className="text-sm text-gray-600 mt-1">Browse and accept photography orders from real estate agents</p>
          </div>
        </div>
      </div>

      {/* Filters Card */}
      <Card className="shadow-md">
        <CardContent className="pt-6">
          <OrdersFilters
            searchKeyword={searchQuery}
            onSearchChange={setSearchQuery}
            statusFilter={statusFilter}
            onStatusChange={setStatusFilter}
          />
        </CardContent>
      </Card>

      {/* Orders Table Card */}
      <Card className="shadow-lg">
        <CardContent className="p-0">
          {!isLoading && (!data?.items || data.items.length === 0) ? (
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-16 w-16 rounded-full bg-green-50 flex items-center justify-center mb-4">
                <ShoppingBag className="h-8 w-8 text-green-600" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">No orders found</h3>
              <p className="text-sm text-gray-600">
                {searchQuery || statusFilter !== OrderStatus.Placed
                  ? "Try adjusting your filters to find what you're looking for"
                  : "New orders from agents will appear here when available"}
              </p>
            </div>
          ) : (
            <OrdersTable
              orders={data?.items || []}
              isLoading={isLoading}
              searchQuery={searchQuery}
              statusFilter={statusFilter === "all" ? undefined : statusFilter.toString()}
              isMarketplace
            />
          )}
        </CardContent>
      </Card>
    </div>
  );
}
