"use client";

import { useState } from "react";
import Link from "next/link";
import { PageHeader, LoadingState, ErrorState, ConfirmDialog, Pagination } from "@/components/shared";
import { OrdersTable, OrdersFilters } from "@/components/orders";
import { Button } from "@/components/ui/button";
import { useOrders, useCancelOrder } from "@/lib/hooks/use-orders";
import { OrderStatus } from "@/types";
import { Plus } from "lucide-react";

export default function AgentOrdersPage() {
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "all">("all");
  const [searchKeyword, setSearchKeyword] = useState("");
  const [page, setPage] = useState(1);
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);

  const { data, isLoading, error } = useOrders({
    status: statusFilter === "all" ? undefined : statusFilter,
    keyword: searchKeyword || undefined,
    page,
    pageSize: 20,
  });

  const cancelMutation = useCancelOrder();

  const handleStatusChange = (newStatus: OrderStatus | "all") => {
    setStatusFilter(newStatus);
    setPage(1);
  };

  const handleSearchChange = (newKeyword: string) => {
    setSearchKeyword(newKeyword);
    setPage(1);
  };

  const handleCancel = () => {
    if (selectedOrderId) {
      cancelMutation.mutate({ id: selectedOrderId });
      setCancelDialogOpen(false);
      setSelectedOrderId(null);
    }
  };

  const openCancelDialog = (id: string) => {
    setSelectedOrderId(id);
    setCancelDialogOpen(true);
  };

  if (isLoading) {
    return <LoadingState />;
  }

  if (error) {
    return <ErrorState message="Failed to load orders" />;
  }

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

      <OrdersFilters
        searchKeyword={searchKeyword}
        onSearchChange={handleSearchChange}
        statusFilter={statusFilter}
        onStatusChange={handleStatusChange}
      />

      <OrdersTable
        orders={data?.items || []}
        onCancel={openCancelDialog}
      />

      {data && (
        <Pagination
          currentPage={data.page}
          totalItems={data.total}
          pageSize={data.pageSize}
          onPageChange={setPage}
        />
      )}

      <ConfirmDialog
        open={cancelDialogOpen}
        onOpenChange={setCancelDialogOpen}
        title="Cancel Order"
        description="Are you sure you want to cancel this order? This action cannot be undone."
        onConfirm={handleCancel}
        confirmText="Cancel Order"
        variant="destructive"
      />
    </div>
  );
}
