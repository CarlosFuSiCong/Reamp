"use client";

import { useQuery } from "@tanstack/react-query";
import { Camera, Calendar, CheckCircle, Clock } from "lucide-react";
import { ordersApi } from "@/lib/api";
import { PageHeader } from "@/components/shared/page-header";
import { LoadingState } from "@/components/shared/loading-state";
import { ErrorState } from "@/components/shared/error-state";
import { StatsCard } from "@/components/dashboard";
import { OrderStatus } from "@/types";

export default function StudioDashboardPage() {
  const { data: ordersData, isLoading, error } = useQuery({
    queryKey: ["orders", { page: 1, pageSize: 100 }],
    queryFn: () => ordersApi.list({ page: 1, pageSize: 100 }),
  });

  if (isLoading) {
    return <LoadingState message="Loading dashboard..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load dashboard data" />;
  }

  const orders = ordersData?.items || [];
  
  const pendingOrders = orders.filter(o => o.status === OrderStatus.Placed).length;
  const acceptedOrders = orders.filter(o => o.status === OrderStatus.Accepted).length;
  const completedOrders = orders.filter(o => o.status === OrderStatus.Completed).length;

  return (
    <div>
      <PageHeader
        title="Studio Dashboard"
        description="Welcome back! Manage your photography orders and schedule"
      />

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <StatsCard
          title="Pending Orders"
          value={pendingOrders}
          icon={Clock}
          description="Awaiting acceptance"
        />
        <StatsCard
          title="Active Orders"
          value={acceptedOrders}
          icon={Camera}
          description="In progress"
        />
        <StatsCard
          title="Completed"
          value={completedOrders}
          icon={CheckCircle}
          description="This month"
        />
        <StatsCard
          title="Schedule"
          value={0}
          icon={Calendar}
          description="Upcoming shoots"
        />
      </div>

      <div className="flex items-center justify-center h-64 border-2 border-dashed border-gray-300 rounded-lg">
        <p className="text-muted-foreground">Studio features coming soon...</p>
      </div>
    </div>
  );
}

