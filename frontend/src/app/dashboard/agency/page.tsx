"use client";

import { Home, ShoppingCart, Clock, Plus, Calendar, Package, Building2 } from "lucide-react";
import { LoadingState, ErrorState } from "@/components/shared";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
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
    <div className="space-y-6">
      {/* Header with Icon */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-blue-600 flex items-center justify-center text-white shadow-md">
              <Building2 className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">Agency Dashboard</h1>
              <p className="text-sm text-gray-600 mt-1">Welcome back! Here's an overview of your activities</p>
            </div>
          </div>
        </div>
        <Button asChild className="shadow-md">
          <Link href="/dashboard/listings/new">
            <Plus className="h-4 w-4 mr-2" />
            New Listing
          </Link>
        </Button>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <Card className="shadow-md hover:shadow-lg transition-shadow">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium text-gray-600">Total Listings</CardTitle>
            <div className="h-9 w-9 rounded-lg bg-blue-50 flex items-center justify-center">
              <Home className="h-4 w-4 text-blue-600" />
            </div>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalListings}</div>
            <p className="text-xs text-gray-600 mt-1">Active properties</p>
          </CardContent>
        </Card>

        <Card className="shadow-md hover:shadow-lg transition-shadow">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium text-gray-600">Active Orders</CardTitle>
            <div className="h-9 w-9 rounded-lg bg-purple-50 flex items-center justify-center">
              <ShoppingCart className="h-4 w-4 text-purple-600" />
            </div>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.activeOrders}</div>
            <p className="text-xs text-gray-600 mt-1">In progress</p>
          </CardContent>
        </Card>

        <Card className="shadow-md hover:shadow-lg transition-shadow">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium text-gray-600">Pending Review</CardTitle>
            <div className="h-9 w-9 rounded-lg bg-orange-50 flex items-center justify-center">
              <Clock className="h-4 w-4 text-orange-600" />
            </div>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.pendingListings}</div>
            <p className="text-xs text-gray-600 mt-1">Awaiting approval</p>
          </CardContent>
        </Card>
      </div>

      {/* Activity Section */}
      <Card className="shadow-lg">
        <CardHeader>
          <CardTitle>Recent Activity</CardTitle>
          <CardDescription>Latest updates from your account</CardDescription>
        </CardHeader>
        <CardContent>
          {recentActivities.length > 0 ? (
            <div className="space-y-4">
              {recentActivities.map((activity, index) => (
                <div
                  key={index}
                  className="flex items-start gap-4 p-3 rounded-lg hover:bg-gray-50 transition-colors border border-transparent hover:border-gray-200"
                >
                  <div className={`rounded-lg p-2 ${
                    activity.type === 'listing' ? 'bg-blue-50 text-blue-600' :
                    activity.type === 'order' ? 'bg-purple-50 text-purple-600' :
                    'bg-green-50 text-green-600'
                  }`}>
                    {activity.type === 'listing' ? <Home className="h-4 w-4" /> :
                     activity.type === 'order' ? <ShoppingCart className="h-4 w-4" /> :
                     <Package className="h-4 w-4" />}
                  </div>
                  <div className="flex-1 space-y-1">
                    <p className="text-sm font-semibold text-gray-900">{activity.title}</p>
                    <p className="text-sm text-gray-600">{activity.description}</p>
                    <div className="flex items-center gap-2 pt-1">
                      <Calendar className="h-3 w-3 text-gray-400" />
                      <p className="text-xs text-gray-500">{activity.time}</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12 text-gray-500">
              <div className="mx-auto h-16 w-16 rounded-full bg-gray-100 flex items-center justify-center mb-4">
                <Calendar className="h-8 w-8 text-gray-400" />
              </div>
              <p className="text-base font-semibold text-gray-900">No recent activity</p>
              <p className="text-sm mt-1 text-gray-600">Your activity will appear here</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
