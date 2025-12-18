"use client";

import { Home, ShoppingCart, Clock, Plus, TrendingUp, ArrowRight, Calendar, Package } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useDashboardStats } from "@/lib/hooks/use-dashboard-stats";
import { generateRecentActivities } from "@/lib/utils/activity-utils";
import Link from "next/link";

export default function AgentDashboardPage() {
  const { stats, listings, orders, isLoading, error } = useDashboardStats();

  if (isLoading) {
    return <LoadingState message="Loading dashboard..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load dashboard data" />;
  }

  const recentActivities = generateRecentActivities(listings, orders, 5);

  return (
    <div className="space-y-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-gray-900">Agency Dashboard</h1>
          <p className="text-muted-foreground mt-1">
            Welcome back! Here's an overview of your activities
          </p>
        </div>
        <Button asChild className="shadow-lg">
          <Link href="/dashboard/listings/new">
            <Plus className="h-4 w-4 mr-2" />
            New Listing
          </Link>
        </Button>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <StatsCard
          title="Total Listings"
          value={stats.totalListings.toString()}
          description="Active properties"
          icon={Home}
          color="blue"
          trend="+12%"
        />
        <StatsCard
          title="Active Orders"
          value={stats.activeOrders.toString()}
          description="In progress"
          icon={ShoppingCart}
          color="purple"
        />
        <StatsCard
          title="Pending Review"
          value={stats.pendingListings.toString()}
          description="Awaiting approval"
          icon={Clock}
          color="orange"
        />
      </div>

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Quick Actions */}
        <Card className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300 lg:col-span-1">
          <CardHeader>
            <CardTitle className="text-xl">Quick Actions</CardTitle>
            <CardDescription>Frequently used actions</CardDescription>
          </CardHeader>
          <CardContent className="grid gap-3">
            <Link href="/dashboard/listings/new">
              <div className="group relative overflow-hidden rounded-xl border-2 border-blue-100 bg-gradient-to-br from-blue-50 to-indigo-50 p-4 hover:border-blue-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                <div className="flex items-center gap-3">
                  <div className="rounded-lg bg-blue-600 p-2 text-white group-hover:scale-110 transition-transform">
                    <Plus className="h-4 w-4" />
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900">Create Listing</p>
                    <p className="text-xs text-gray-600">Add new property</p>
                  </div>
                  <ArrowRight className="h-4 w-4 text-blue-600 ml-auto opacity-0 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
                </div>
              </div>
            </Link>

            <Link href="/dashboard/orders/new">
              <div className="group relative overflow-hidden rounded-xl border-2 border-purple-100 bg-gradient-to-br from-purple-50 to-pink-50 p-4 hover:border-purple-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                <div className="flex items-center gap-3">
                  <div className="rounded-lg bg-purple-600 p-2 text-white group-hover:scale-110 transition-transform">
                    <ShoppingCart className="h-4 w-4" />
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900">New Order</p>
                    <p className="text-xs text-gray-600">Book a shoot</p>
                  </div>
                  <ArrowRight className="h-4 w-4 text-purple-600 ml-auto opacity-0 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
                </div>
              </div>
            </Link>

            <Link href="/dashboard/listings">
              <div className="group relative overflow-hidden rounded-xl border-2 border-green-100 bg-gradient-to-br from-green-50 to-emerald-50 p-4 hover:border-green-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                <div className="flex items-center gap-3">
                  <div className="rounded-lg bg-green-600 p-2 text-white group-hover:scale-110 transition-transform">
                    <Home className="h-4 w-4" />
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900">All Listings</p>
                    <p className="text-xs text-gray-600">View properties</p>
                  </div>
                  <ArrowRight className="h-4 w-4 text-green-600 ml-auto opacity-0 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
                </div>
              </div>
            </Link>
          </CardContent>
        </Card>

        {/* Recent Activity */}
        <Card className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300 lg:col-span-2">
          <CardHeader>
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-xl">Recent Activity</CardTitle>
                <CardDescription>Latest updates from your account</CardDescription>
              </div>
              <Button variant="ghost" size="sm" className="text-blue-600 hover:text-blue-700">
                View All
                <ArrowRight className="h-4 w-4 ml-2" />
              </Button>
            </div>
          </CardHeader>
          <CardContent>
            {recentActivities.length > 0 ? (
              <div className="space-y-4">
                {recentActivities.map((activity, index) => (
                  <div
                    key={index}
                    className="flex items-start gap-4 p-3 rounded-lg hover:bg-gray-50 transition-colors group"
                  >
                    <div className={`rounded-xl p-2.5 ${
                      activity.type === 'listing' ? 'bg-blue-50 text-blue-600' :
                      activity.type === 'order' ? 'bg-purple-50 text-purple-600' :
                      'bg-green-50 text-green-600'
                    } group-hover:scale-110 transition-transform`}>
                      {activity.type === 'listing' ? <Home className="h-4 w-4" /> :
                       activity.type === 'order' ? <ShoppingCart className="h-4 w-4" /> :
                       <Package className="h-4 w-4" />}
                    </div>
                    <div className="flex-1 space-y-1">
                      <p className="text-sm font-semibold text-gray-900 group-hover:text-blue-600 transition-colors">
                        {activity.title}
                      </p>
                      <p className="text-sm text-gray-600">{activity.description}</p>
                      <div className="flex items-center gap-2 pt-1">
                        <Calendar className="h-3 w-3 text-gray-400" />
                        <p className="text-xs text-gray-500 font-medium">{activity.time}</p>
                      </div>
                    </div>
                    {activity.status && (
                      <Badge variant="outline" className="text-xs">
                        {activity.status}
                      </Badge>
                    )}
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-12 text-gray-500">
                <Calendar className="mx-auto h-12 w-12 mb-4 text-gray-400" />
                <p className="text-lg font-medium">No recent activity</p>
                <p className="text-sm mt-1">Your activity will appear here</p>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function StatsCard({
  title,
  value,
  description,
  icon: Icon,
  color = "blue",
  trend,
}: {
  title: string;
  value: string;
  description: string;
  icon: any;
  color?: "blue" | "green" | "purple" | "orange";
  trend?: string;
}) {
  const colorClasses = {
    blue: { light: "bg-blue-50", icon: "text-blue-600" },
    green: { light: "bg-green-50", icon: "text-green-600" },
    purple: { light: "bg-purple-50", icon: "text-purple-600" },
    orange: { light: "bg-orange-50", icon: "text-orange-600" },
  };

  const colors = colorClasses[color];

  return (
    <Card className="relative overflow-hidden border-0 shadow-md hover:shadow-xl transition-all duration-300 group cursor-pointer hover:-translate-y-1">
      <div className="absolute top-0 right-0 w-32 h-32 opacity-5">
        <Icon className="w-full h-full" />
      </div>

      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
        <CardTitle className="text-sm font-semibold text-gray-600">{title}</CardTitle>
        <div className={`rounded-xl p-2.5 ${colors.light} ${colors.icon} group-hover:scale-110 transition-transform`}>
          <Icon className="h-5 w-5" />
        </div>
      </CardHeader>
      <CardContent className="space-y-2">
        <div className="text-3xl font-bold text-gray-900">{value}</div>
        <div className="flex items-center gap-2">
          {trend && (
            <div className="flex items-center gap-1 px-2 py-1 rounded-full text-xs font-semibold bg-green-50 text-green-700">
              <TrendingUp className="h-3 w-3" />
              {trend}
            </div>
          )}
          <p className="text-xs text-gray-600 font-medium">{description}</p>
        </div>
      </CardContent>
    </Card>
  );
}
