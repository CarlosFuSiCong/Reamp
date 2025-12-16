"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { PageHeader } from "@/components/shared";
import { OrdersTable, OrdersFilters } from "@/components/orders";
import { useProfile } from "@/lib/hooks";
import { ordersApi } from "@/lib/api";
import { OrderStatus } from "@/types";

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
        searchKeyword={searchQuery}
        onSearchChange={setSearchQuery}
        statusFilter={statusFilter}
        onStatusChange={setStatusFilter}
      />

      <OrdersTable
        orders={data?.items || []}
        isLoading={isLoading}
        searchQuery={searchQuery}
        statusFilter={statusFilter === "all" ? undefined : statusFilter.toString()}
        isMarketplace
      />
    </div>
  );
}
