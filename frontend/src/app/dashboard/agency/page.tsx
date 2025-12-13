"use client";

import { Home, ShoppingCart, Users, Clock, Plus } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { StatsCard, ActivityTimeline, QuickActions } from "@/components/dashboard";
import { useDashboardStats } from "@/lib/hooks/use-dashboard-stats";
import { generateRecentActivities } from "@/lib/utils/activity-utils";

export default function AgentDashboardPage() {
  const { stats, listings, orders, isLoading, error } = useDashboardStats();

  if (isLoading) {
    return <LoadingState message="Loading dashboard..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load dashboard data" />;
  }

  const recentActivities = generateRecentActivities(listings, orders, 5);

  const quickActions = [
    { href: "/dashboard/agency/listings/new", label: "Create New Listing", icon: Plus },
    { href: "/dashboard/agency/orders/new", label: "Create New Order", icon: Plus },
    { href: "/dashboard/agency/listings", label: "View All Listings", icon: Home },
  ];

  return (
    <div>
      <PageHeader
        title="Agency Dashboard"
        description="Welcome back! Here's an overview of your activities"
      />

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <StatsCard
          title="Total Listings"
          value={stats.totalListings}
          icon={Home}
          description="Active properties"
        />
        <StatsCard
          title="Active Orders"
          value={stats.activeOrders}
          icon={ShoppingCart}
          description="In progress"
        />
        <StatsCard
          title="Clients"
          value={stats.totalClients}
          icon={Users}
          description="Total clients"
        />
        <StatsCard
          title="Pending Review"
          value={stats.pendingListings}
          icon={Clock}
          description="Awaiting approval"
        />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <QuickActions actions={quickActions} />
        <ActivityTimeline activities={recentActivities} />
      </div>
    </div>
  );
}

