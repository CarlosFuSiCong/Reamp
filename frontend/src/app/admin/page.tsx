"use client";

import { PageHeader } from "@/components/shared/page-header";
import { LoadingState } from "@/components/shared/loading-state";
import { useAdminDashboard } from "@/lib/hooks/use-admin-dashboard";
import { StatsGrid, ActivitySection, ManagementTabs } from "@/components/admin/dashboard";
import { Activity } from "@/lib/api/admin";

export default function AdminDashboardPage() {
  const { stats, activities, isLoading } = useAdminDashboard();

  if (isLoading) {
    return <LoadingState message="Loading admin dashboard..." />;
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Admin Dashboard"
        description="System overview and management"
      />

      <StatsGrid stats={stats} />

      <ActivitySection 
        chartData={stats.chartData} 
        activities={activities} 
      />

      <ManagementTabs alerts={stats.alerts} />
    </div>
  );
}
