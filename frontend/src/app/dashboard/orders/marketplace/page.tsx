"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { PageHeader } from "@/components/shared";
import { OrdersTable } from "@/components/orders/orders-table";
import { OrdersFilters } from "@/components/orders/orders-filters";
import { ordersApi } from "@/lib/api";
import { OrderStatus } from "@/types";
import { useProfile } from "@/lib/hooks";

export default function OrdersMarketplacePage() {
  const { user } = useProfile();
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>(OrderStatus.Placed.toString());

  // Fetch Placed orders (market orders)
  const { data, isLoading } = useQuery({
    queryKey: ["orders", "marketplace", statusFilter, searchQuery],
    queryFn: () =>
      ordersApi.list({
        status: statusFilter === "all" ? undefined : statusFilter,
        keyword: searchQuery || undefined,
        page: 1,
        pageSize: 50,
      }),
    enabled: !!user?.studioId,
  });

  if (!user?.studioId) {
    return (
      <div className="space-y-6">
        <PageHeader
          title="Orders Marketplace"
          description="Only Studio members can access the marketplace"
        />
        <div className="text-center py-12 text-muted-foreground">
          <p>You must be a member of a photography studio to view marketplace orders.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Orders Marketplace"
        description="Browse and accept orders from agents"
      />

      <OrdersFilters
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
        statusFilter={statusFilter}
        onStatusChange={setStatusFilter}
        showOnlyPlaced
      />

      <OrdersTable
        orders={data?.items || []}
        isLoading={isLoading}
        searchQuery={searchQuery}
        statusFilter={statusFilter}
        isMarketplace
      />
    </div>
  );
}
