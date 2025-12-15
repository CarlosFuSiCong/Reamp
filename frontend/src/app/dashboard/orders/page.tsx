"use client";

import { useState } from "react";
import Link from "next/link";
import { PageHeader, LoadingState, ErrorState, ConfirmDialog, Pagination } from "@/components/shared";
import { OrdersTable, OrdersFilters } from "@/components/orders";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useOrders, useCancelOrder, useAvailableOrders, usePhotographerOrders, useAcceptOrder, useStartOrder, useCompleteOrder } from "@/lib/hooks/use-orders";
import { useProfile } from "@/lib/hooks";
import { OrderStatus, ShootOrder, ShootTaskType } from "@/types";
import { Plus, Calendar, Package, DollarSign, Clock } from "lucide-react";
import { format } from "date-fns";

type StaffTabValue = "available" | "my-orders" | "in-progress" | "completed";

export default function AgentOrdersPage() {
  const { user: profile, isLoading: profileLoading } = useProfile();
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "all">("all");
  const [searchKeyword, setSearchKeyword] = useState("");
  const [page, setPage] = useState(1);
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  
  // Staff-specific states
  const [staffTab, setStaffTab] = useState<StaffTabValue>("available");
  const [availablePage, setAvailablePage] = useState(1);
  const [myOrdersPage, setMyOrdersPage] = useState(1);
  const [inProgressPage, setInProgressPage] = useState(1);
  const [completedPage, setCompletedPage] = useState(1);

  // Determine if user is staff
  const isStaff = profile?.studioRole !== undefined && profile?.studioRole !== null;
  const isAgent = profile?.agencyRole !== undefined && profile?.agencyRole !== null;

  // Agent queries
  const { data: agentOrders, isLoading: agentLoading, error: agentError } = useOrders({
    status: statusFilter === "all" ? undefined : statusFilter,
    keyword: searchKeyword || undefined,
    page,
    pageSize: 20,
  });

  // Staff queries
  const { data: availableData, isLoading: availableLoading, error: availableError } = useAvailableOrders({
    page: availablePage,
    pageSize: 20,
  });

  // My Orders: Accepted + Scheduled (orders assigned to this photographer but not started yet)
  const { data: myOrdersData, isLoading: myOrdersLoading, error: myOrdersError } = usePhotographerOrders({
    status: undefined, // Get all statuses, will filter on backend
    page: myOrdersPage,
    pageSize: 20,
  });

  const { data: inProgressData, isLoading: inProgressLoading, error: inProgressError } = usePhotographerOrders({
    status: OrderStatus.InProgress,
    page: inProgressPage,
    pageSize: 20,
  });

  const { data: completedData, isLoading: completedLoading, error: completedError } = usePhotographerOrders({
    status: OrderStatus.Completed,
    page: completedPage,
    pageSize: 20,
  });

  const cancelMutation = useCancelOrder();
  const acceptMutation = useAcceptOrder();
  const startMutation = useStartOrder();
  const completeMutation = useCompleteOrder();

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

  const handleAccept = (orderId: string) => {
    acceptMutation.mutate(orderId);
  };

  const handleStart = (orderId: string) => {
    startMutation.mutate(orderId);
  };

  const handleComplete = (orderId: string) => {
    completeMutation.mutate(orderId);
  };

  const getStatusBadge = (status: OrderStatus) => {
    const variants: Record<OrderStatus, { label: string; variant: "default" | "secondary" | "destructive" | "outline" }> = {
      [OrderStatus.Placed]: { label: "Placed", variant: "secondary" },
      [OrderStatus.Accepted]: { label: "Accepted", variant: "default" },
      [OrderStatus.Scheduled]: { label: "Scheduled", variant: "default" },
      [OrderStatus.InProgress]: { label: "In Progress", variant: "default" },
      [OrderStatus.Completed]: { label: "Completed", variant: "outline" },
      [OrderStatus.Cancelled]: { label: "Cancelled", variant: "destructive" },
    };

    const config = variants[status];
    return <Badge variant={config.variant}>{config.label}</Badge>;
  };

  const getTaskTypeName = (type: ShootTaskType): string => {
    const names: Record<ShootTaskType, string> = {
      [ShootTaskType.None]: "None",
      [ShootTaskType.Photography]: "Photography",
      [ShootTaskType.Video]: "Video",
      [ShootTaskType.Floorplan]: "Floor Plan",
    };
    return names[type] || "Unknown";
  };

  const renderOrderCard = (order: ShootOrder, showActions: boolean = true) => (
    <Card key={order.id} className="hover:shadow-md transition-shadow">
      <CardHeader>
        <div className="flex items-start justify-between">
          <div className="space-y-1">
            <CardTitle className="text-lg">
              Order #{order.id.substring(0, 8)}
            </CardTitle>
            <CardDescription className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              {format(new Date(order.createdAtUtc), "PPP")}
            </CardDescription>
          </div>
          {getStatusBadge(order.status)}
        </div>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-4 text-sm">
            <div className="flex items-center gap-2">
              <Package className="h-4 w-4 text-muted-foreground" />
              <span>
                {order.taskCount ?? order.tasks?.length ?? 0} task
                {(order.taskCount ?? order.tasks?.length ?? 0) !== 1 ? 's' : ''}
              </span>
            </div>
            <div className="flex items-center gap-2">
              <DollarSign className="h-4 w-4 text-muted-foreground" />
              <span className="font-semibold">{order.currency} {order.totalAmount.toFixed(2)}</span>
            </div>
          </div>

          {order.scheduledStartUtc && (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Clock className="h-4 w-4" />
              <span>Scheduled: {format(new Date(order.scheduledStartUtc), "PPP p")}</span>
            </div>
          )}

          {showActions && (
            <div className="flex gap-2 pt-2">
              <Button variant="outline" size="sm" asChild className="flex-1">
                <Link href={`/dashboard/orders/${order.id}`}>
                  View Details
                </Link>
              </Button>
              
              {staffTab === "available" && (
                <Button 
                  size="sm" 
                  className="flex-1"
                  onClick={() => handleAccept(order.id)}
                  disabled={acceptMutation.isPending}
                >
                  Accept Order
                </Button>
              )}
              
              {staffTab === "my-orders" && order.status === OrderStatus.Accepted && (
                <Button 
                  size="sm" 
                  className="flex-1"
                  onClick={() => handleStart(order.id)}
                  disabled={startMutation.isPending}
                >
                  Start Shoot
                </Button>
              )}
              
              {staffTab === "my-orders" && order.status === OrderStatus.Scheduled && (
                <Button 
                  size="sm" 
                  className="flex-1"
                  onClick={() => handleStart(order.id)}
                  disabled={startMutation.isPending}
                >
                  Start Shoot
                </Button>
              )}
              
              {staffTab === "in-progress" && (
                <Button 
                  size="sm" 
                  className="flex-1"
                  onClick={() => handleComplete(order.id)}
                  disabled={completeMutation.isPending}
                >
                  Complete
                </Button>
              )}
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );

  const renderStaffTabContent = (
    data: typeof availableData,
    isLoading: boolean,
    error: any,
    page: number,
    setPage: (page: number) => void,
    emptyMessage: string
  ) => {
    if (isLoading) {
      return <LoadingState />;
    }

    if (error) {
      return <ErrorState message="Failed to load orders" />;
    }

    if (!data || data.items.length === 0) {
      return (
        <div className="text-center py-12">
          <p className="text-muted-foreground">{emptyMessage}</p>
        </div>
      );
    }

    return (
      <div className="space-y-4">
        <div className="grid gap-4 md:grid-cols-2">
          {data.items.map(order => renderOrderCard(order))}
        </div>
        
        <Pagination
          currentPage={data.page}
          totalItems={data.total}
          pageSize={data.pageSize}
          onPageChange={setPage}
        />
      </div>
    );
  };

  if (profileLoading) {
    return <LoadingState />;
  }

  // Staff view (Studio members)
  if (isStaff) {
    return (
      <div className="space-y-6">
        <PageHeader
          title="My Orders"
          description="Manage your photography assignments"
        />

        <Tabs value={staffTab} onValueChange={(v) => setStaffTab(v as StaffTabValue)}>
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="available">
              Available ({availableData?.total || 0})
            </TabsTrigger>
            <TabsTrigger value="my-orders">
              My Orders ({myOrdersData?.total || 0})
            </TabsTrigger>
            <TabsTrigger value="in-progress">
              In Progress ({inProgressData?.total || 0})
            </TabsTrigger>
            <TabsTrigger value="completed">
              Completed ({completedData?.total || 0})
            </TabsTrigger>
          </TabsList>

          <TabsContent value="available" className="mt-6">
            {renderStaffTabContent(
              availableData,
              availableLoading,
              availableError,
              availablePage,
              setAvailablePage,
              "No available orders at the moment"
            )}
          </TabsContent>

          <TabsContent value="my-orders" className="mt-6">
            {renderStaffTabContent(
              myOrdersData,
              myOrdersLoading,
              myOrdersError,
              myOrdersPage,
              setMyOrdersPage,
              "No orders assigned to you yet"
            )}
          </TabsContent>

          <TabsContent value="in-progress" className="mt-6">
            {renderStaffTabContent(
              inProgressData,
              inProgressLoading,
              inProgressError,
              inProgressPage,
              setInProgressPage,
              "No orders in progress"
            )}
          </TabsContent>

          <TabsContent value="completed" className="mt-6">
            {renderStaffTabContent(
              completedData,
              completedLoading,
              completedError,
              completedPage,
              setCompletedPage,
              "No completed orders"
            )}
          </TabsContent>
        </Tabs>
      </div>
    );
  }

  // Agent view (default)
  if (agentLoading) {
    return <LoadingState />;
  }

  if (agentError) {
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
        orders={agentOrders?.items || []}
        onCancel={openCancelDialog}
      />

      {agentOrders && (
        <Pagination
          currentPage={agentOrders.page}
          totalItems={agentOrders.total}
          pageSize={agentOrders.pageSize}
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
