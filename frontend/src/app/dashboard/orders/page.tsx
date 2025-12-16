"use client";

import { useState } from "react";
import Link from "next/link";
import {
  PageHeader,
  LoadingState,
  ErrorState,
  ConfirmDialog,
  Pagination,
} from "@/components/shared";
import { OrdersTable, OrdersFilters } from "@/components/orders";
import { Button } from "@/components/ui/button";
import { useOrders } from "@/lib/hooks/use-orders";
import { useProfile } from "@/lib/hooks";
import { OrderStatus } from "@/types";
import { Plus } from "lucide-react";

export default function OrdersPage() {
  const { user: profile, isLoading: profileLoading } = useProfile();
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "all">("all");
  const [searchKeyword, setSearchKeyword] = useState("");
  const [page, setPage] = useState(1);
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);

  // Check user role
  const isAgent = profile?.agencyRole !== undefined && profile?.agencyRole !== null;
  const isStudio = profile?.studioRole !== undefined && profile?.studioRole !== null;

  // Query orders based on user role
  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useOrders({
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

  const openCancelDialog = (id: string) => {
    setSelectedOrderId(id);
    setCancelDialogOpen(true);
  };

  if (profileLoading) {
    return <LoadingState message="Loading orders..." />;
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

      <OrdersFilters
        searchKeyword={searchKeyword}
        onSearchChange={handleSearchChange}
        statusFilter={statusFilter}
        onStatusChange={handleStatusChange}
      />

      <OrdersTable
        orders={ordersData?.items || []}
        onCancel={isAgent ? openCancelDialog : undefined}
      />

      {ordersData && (
        <Pagination
          currentPage={ordersData.page}
          totalItems={ordersData.total}
          pageSize={ordersData.pageSize}
          onPageChange={setPage}
        />
      )}

      {isAgent && (
        <ConfirmDialog
          open={cancelDialogOpen}
          onOpenChange={setCancelDialogOpen}
          title="Cancel Order"
          description="Are you sure you want to cancel this order? This action cannot be undone."
          onConfirm={() => {
            // Cancel mutation will be handled by OrderActions component
            setCancelDialogOpen(false);
            setSelectedOrderId(null);
          }}
          confirmText="Cancel Order"
          variant="destructive"
        />
      )}
    </div>
  );
}
