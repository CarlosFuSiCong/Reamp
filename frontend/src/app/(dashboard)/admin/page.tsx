"use client";

import { PageHeader } from "@/components/shared/page-header";
import { StatsCard } from "@/components/dashboard/stats-card";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { LoadingState } from "@/components/shared/loading-state";
import { ErrorState } from "@/components/shared/error-state";
import { useAdminStats } from "@/lib/hooks/use-admin-stats";
import { useApplications } from "@/lib/hooks";
import { ApplicationStatus } from "@/types";
import { Users, Package, ShoppingCart, Building2, TrendingUp, AlertCircle, ClipboardCheck } from "lucide-react";
import { ActivityTimeline, Activity } from "@/components/dashboard/activity-timeline";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { UsersTable, AgenciesTable, StudiosTable } from "@/components/admin";
import { ApplicationsList } from "@/components/applications";
import { StatsChart } from "@/components/admin/stats-chart";
import { Activity as AdminActivity } from "@/lib/api/admin";

export default function AdminDashboardPage() {
  const { stats, activities, isLoading, error } = useAdminStats();
  const { data: pendingApps } = useApplications(1, 10, ApplicationStatus.Pending);

  if (isLoading) {
    return <LoadingState message="Loading admin dashboard..." />;
  }

  // Use default values if stats fail to load
  const safeStats = error ? {
    totalUsers: 0,
    activeListings: 0,
    totalOrders: 0,
    totalStudios: 0,
    chartData: [],
    alerts: []
  } : stats;

  const safeActivities = error ? [] : activities;

  return (
    <div className="space-y-6">
      <PageHeader
        title="Admin Dashboard"
        description="System overview and management"
      />

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-5">
        <StatsCard
          title="Pending Applications"
          value={pendingApps?.total ?? 0}
          icon={ClipboardCheck}
          description="Awaiting review"
          className="border-orange-200 bg-orange-50"
        />
        <StatsCard
          title="Total Users"
          value={safeStats.totalUsers}
          icon={Users}
          description="Active platform users"
        />
        <StatsCard
          title="Active Listings"
          value={safeStats.activeListings}
          icon={Package}
          description="Currently active"
        />
        <StatsCard
          title="Total Orders"
          value={safeStats.totalOrders}
          icon={ShoppingCart}
          description="All time orders"
        />
        <StatsCard
          title="Studios"
          value={safeStats.totalStudios}
          icon={Building2}
          description="Registered studios"
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-7">
        <Card className="lg:col-span-4">
          <CardHeader>
            <CardTitle>System Activity</CardTitle>
          </CardHeader>
          <CardContent>
            <StatsChart data={safeStats.chartData} />
          </CardContent>
        </Card>

        <Card className="lg:col-span-3">
          <CardHeader>
            <CardTitle>Recent Activity</CardTitle>
          </CardHeader>
          <CardContent>
            <ActivityTimeline activities={safeActivities as Activity[]} />
          </CardContent>
        </Card>
      </div>

      <Tabs defaultValue="applications" className="space-y-4">
        <TabsList>
          <TabsTrigger value="applications">Applications</TabsTrigger>
          <TabsTrigger value="users">Users</TabsTrigger>
          <TabsTrigger value="agencies">Agencies</TabsTrigger>
          <TabsTrigger value="studios">Studios</TabsTrigger>
          <TabsTrigger value="alerts">System Alerts</TabsTrigger>
        </TabsList>

        <TabsContent value="users" className="space-y-4">
          <UsersTable />
        </TabsContent>

        <TabsContent value="agencies" className="space-y-4">
          <AgenciesTable />
        </TabsContent>

        <TabsContent value="studios" className="space-y-4">
          <StudiosTable />
        </TabsContent>

        <TabsContent value="applications" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Organization Applications</CardTitle>
            </CardHeader>
            <CardContent>
              <ApplicationsList />
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="alerts" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertCircle className="h-5 w-5" />
                System Alerts
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                {safeStats.alerts?.length > 0 ? (
                  safeStats.alerts.map((alert, index) => (
                    <div
                      key={index}
                      className="flex items-start gap-3 rounded-lg border p-3"
                    >
                      <AlertCircle className="h-5 w-5 text-yellow-600 mt-0.5" />
                      <div className="flex-1">
                        <p className="text-sm font-medium">{alert.title}</p>
                        <p className="text-xs text-muted-foreground">
                          {alert.message}
                        </p>
                      </div>
                    </div>
                  ))
                ) : (
                  <p className="text-sm text-muted-foreground text-center py-4">
                    No system alerts
                  </p>
                )}
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
