"use client";

import { useQuery } from "@tanstack/react-query";
import { Home, ShoppingCart, Users, Clock, Plus } from "lucide-react";
import Link from "next/link";
import { listingsApi, ordersApi } from "@/lib/api";
import { PageHeader } from "@/components/shared/page-header";
import { LoadingState } from "@/components/shared/loading-state";
import { ErrorState } from "@/components/shared/error-state";
import { StatsCard, ActivityTimeline, Activity } from "@/components/dashboard";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export default function AgentDashboardPage() {
  const { data: listingsData, isLoading: loadingListings, error: listingsError } = useQuery({
    queryKey: ["listings", { page: 1, pageSize: 100 }],
    queryFn: () => listingsApi.list({ page: 1, pageSize: 100 }),
  });

  const { data: ordersData, isLoading: loadingOrders, error: ordersError } = useQuery({
    queryKey: ["orders", { page: 1, pageSize: 100 }],
    queryFn: () => ordersApi.list({ page: 1, pageSize: 100 }),
  });

  if (loadingListings || loadingOrders) {
    return <LoadingState message="Loading dashboard..." />;
  }

  if (listingsError || ordersError) {
    return <ErrorState message="Failed to load dashboard data" />;
  }

  const listings = listingsData?.items || [];
  const orders = ordersData?.items || [];
  
  const totalListings = listings.length;
  const activeOrders = orders.filter(o => o.status === "Pending" || o.status === "Accepted").length;
  const pendingListings = listings.filter(l => l.status === "Draft" || l.status === "PendingReview").length;

  const recentActivities: Activity[] = [
    ...orders.slice(0, 3).map(order => ({
      id: order.id,
      type: "order" as const,
      title: "New Order Created",
      description: `Order ${order.id.substring(0, 8)} has been created`,
      timestamp: new Date(order.createdAtUtc).toLocaleString(),
    })),
    ...listings.slice(0, 2).map(listing => ({
      id: listing.id,
      type: "listing" as const,
      title: "Listing Updated",
      description: listing.title,
      timestamp: "Just now",
    })),
  ].sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()).slice(0, 5);

  return (
    <div>
      <PageHeader
        title="Dashboard"
        description="Welcome back! Here's an overview of your activities"
      />

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <StatsCard
          title="Total Listings"
          value={totalListings}
          icon={Home}
          description="Active properties"
        />
        <StatsCard
          title="Active Orders"
          value={activeOrders}
          icon={ShoppingCart}
          description="In progress"
        />
        <StatsCard
          title="Clients"
          value={0}
          icon={Users}
          description="Total clients"
        />
        <StatsCard
          title="Pending Review"
          value={pendingListings}
          icon={Clock}
          description="Awaiting approval"
        />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Quick Actions</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-2">
            <Link href="/agent/listings/new">
              <Button className="w-full justify-start" variant="outline">
                <Plus className="mr-2 h-4 w-4" />
                Create New Listing
              </Button>
            </Link>
            <Link href="/agent/orders/new">
              <Button className="w-full justify-start" variant="outline">
                <Plus className="mr-2 h-4 w-4" />
                Create New Order
              </Button>
            </Link>
            <Link href="/agent/listings">
              <Button className="w-full justify-start" variant="outline">
                <Home className="mr-2 h-4 w-4" />
                View All Listings
              </Button>
            </Link>
          </CardContent>
        </Card>

        <ActivityTimeline activities={recentActivities} />
      </div>
    </div>
  );
}

