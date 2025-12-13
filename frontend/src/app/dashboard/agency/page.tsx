"use client";

import { useState } from "react";
import { Home, ShoppingCart, Users, Clock, Plus, UserPlus } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { StatsCard, ActivityTimeline, QuickActions } from "@/components/dashboard";
import { InviteMemberDialog } from "@/components/agencies";
import { Button } from "@/components/ui/button";
import { useDashboardStats } from "@/lib/hooks/use-dashboard-stats";
import { useProfile } from "@/lib/hooks";
import { generateRecentActivities } from "@/lib/utils/activity-utils";
import { AgencyRole } from "@/types/enums";

export default function AgentDashboardPage() {
  const { stats, listings, orders, isLoading, error } = useDashboardStats();
  const { user: profile } = useProfile();
  const [inviteDialogOpen, setInviteDialogOpen] = useState(false);

  // Check if user can invite (Owner or Manager)
  const canInvite = profile?.agencyRole === AgencyRole.Owner || profile?.agencyRole === AgencyRole.Manager;

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
        action={
          canInvite && (
            <Button onClick={() => setInviteDialogOpen(true)}>
              <UserPlus className="mr-2 h-4 w-4" />
              Invite Member
            </Button>
          )
        }
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

      {canInvite && profile?.agencyId && (
        <InviteMemberDialog
          agencyId={profile.agencyId}
          open={inviteDialogOpen}
          onOpenChange={setInviteDialogOpen}
        />
      )}
    </div>
  );
}

