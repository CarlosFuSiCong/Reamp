"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { PageHeader } from "@/components/shared";
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
        {/* Header */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-green-600 to-emerald-700 flex items-center justify-center text-white">
                <Store className="h-5 w-5" />
              </div>
              <div>
                <h1 className="text-3xl font-bold tracking-tight text-gray-900">Orders Marketplace</h1>
                <p className="text-muted-foreground mt-0.5">
                  Browse and accept orders from agents
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Access Restricted Card */}
        <Card className="border-0 shadow-lg">
          <CardContent className="p-0">
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-24 w-24 rounded-full bg-gradient-to-br from-red-100 to-orange-100 flex items-center justify-center mb-6">
                <Store className="h-12 w-12 text-red-600" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">Studio Access Only</h3>
              <p className="text-gray-600 max-w-md mx-auto">
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
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-green-600 to-emerald-700 flex items-center justify-center text-white">
              <Store className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">Orders Marketplace</h1>
              <p className="text-muted-foreground mt-0.5">
                Browse and accept photography orders from real estate agents
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Filters Card */}
      <Card className="border-0 shadow-md">
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
      <Card className="border-0 shadow-lg">
        <CardContent className="p-0">
          {!isLoading && (!data?.items || data.items.length === 0) ? (
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-24 w-24 rounded-full bg-gradient-to-br from-green-100 to-emerald-100 flex items-center justify-center mb-6">
                <ShoppingBag className="h-12 w-12 text-green-600" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">No orders found</h3>
              <p className="text-gray-600 max-w-md mx-auto">
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
