"use client";

import { useState } from "react";
import { Camera, ShoppingCart, Users, Clock, UserPlus } from "lucide-react";
import { PageHeader } from "@/components/shared";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { InviteStaffDialog } from "@/components/studios";
import { useProfile } from "@/lib/hooks";
import { StudioRole } from "@/types/enums";
import Link from "next/link";

export default function StudioDashboardPage() {
  const { user: profile } = useProfile();
  const [inviteDialogOpen, setInviteDialogOpen] = useState(false);

  // Check if user can invite (Owner or Manager)
  const canInvite = profile?.studioRole === StudioRole.Owner || profile?.studioRole === StudioRole.Manager;

  // Placeholder stats - replace with real data later
  const stats = {
    totalOrders: 0,
    activeOrders: 0,
    completedOrders: 0,
    teamMembers: 0,
  };

  return (
    <div className="space-y-6">
      <PageHeader
        title="Studio Dashboard"
        description="Welcome back! Here's an overview of your studio activities"
        action={
          canInvite && (
            <Button onClick={() => setInviteDialogOpen(true)}>
              <UserPlus className="mr-2 h-4 w-4" />
              Invite Staff
            </Button>
          )
        }
      />

      {/* Stats Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Orders</CardTitle>
            <ShoppingCart className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalOrders}</div>
            <p className="text-xs text-muted-foreground">All time orders</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Orders</CardTitle>
            <Clock className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.activeOrders}</div>
            <p className="text-xs text-muted-foreground">In progress</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Completed</CardTitle>
            <Camera className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.completedOrders}</div>
            <p className="text-xs text-muted-foreground">Successfully delivered</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Team Members</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.teamMembers}</div>
            <p className="text-xs text-muted-foreground">Studio staff</p>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Quick Actions</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {canInvite && (
              <Button 
                variant="outline" 
                className="w-full justify-start"
                onClick={() => setInviteDialogOpen(true)}
              >
                <UserPlus className="mr-2 h-4 w-4" />
                Invite Team Member
              </Button>
            )}
            <Link href="/dashboard/studio/team">
              <Button variant="outline" className="w-full justify-start">
                <Users className="mr-2 h-4 w-4" />
                Manage Team
              </Button>
            </Link>
            <Link href="/dashboard/profile">
              <Button variant="outline" className="w-full justify-start">
                <Camera className="mr-2 h-4 w-4" />
                View Profile
              </Button>
            </Link>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Recent Activity</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-muted-foreground">
              No recent activity to display.
            </p>
          </CardContent>
        </Card>
      </div>

      {canInvite && profile?.studioId && (
        <InviteStaffDialog
          studioId={profile.studioId}
          open={inviteDialogOpen}
          onOpenChange={setInviteDialogOpen}
        />
      )}
    </div>
  );
}
