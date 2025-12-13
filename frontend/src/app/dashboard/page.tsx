"use client";

import { useAuth } from "@/lib/hooks";
import { PageHeader } from "@/components/shared";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { UserRole } from "@/types/enums";
import { Building2, Users, ClipboardList, FileText } from "lucide-react";
import Link from "next/link";
import { Button } from "@/components/ui/button";

export default function DashboardPage() {
  const { user } = useAuth();

  const getWelcomeMessage = () => {
    if (!user) return "Welcome to Reamp";
    
    switch (user.role) {
      case UserRole.AgencyOwner:
        return "Welcome, Agency Owner";
      case UserRole.AgencyManager:
        return "Welcome, Agency Manager";
      case UserRole.AgencyStaff:
        return "Welcome, Agency Staff";
      case UserRole.StudioOwner:
        return "Welcome, Studio Owner";
      case UserRole.StudioManager:
        return "Welcome, Studio Manager";
      case UserRole.StudioStaff:
        return "Welcome, Studio Staff";
      case UserRole.Admin:
        return "Welcome, Administrator";
      default:
        return "Welcome to Reamp";
    }
  };

  // For regular Staff without organization
  if (user?.role === UserRole.Staff) {
    return (
      <div className="space-y-6">
        <PageHeader 
          title="Welcome to Reamp" 
          description="Get started by applying for an organization"
        />

        <Card>
          <CardHeader>
            <CardTitle>Get Started</CardTitle>
            <CardDescription>
              You don't have an organization yet. Apply to create an Agency or Studio to unlock all features.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <Link href="/apply">
              <Button className="w-full" size="lg">
                <Building2 className="mr-2 h-5 w-5" />
                Apply for Organization
              </Button>
            </Link>
            <p className="text-sm text-muted-foreground text-center">
              Once approved, you'll be able to manage teams, clients, and orders.
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Your Profile</CardTitle>
            <CardDescription>Manage your personal information</CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/dashboard/profile">
              <Button variant="outline" className="w-full">
                <FileText className="mr-2 h-4 w-4" />
                Go to Profile
              </Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  // For Agency members
  if ([UserRole.AgencyOwner, UserRole.AgencyManager, UserRole.AgencyStaff].includes(user?.role as UserRole)) {
    const quickActions = [
      { title: "Agency Dashboard", href: "/dashboard/agency", icon: Building2 },
      { title: "Listings", href: "/dashboard/agency/listings", icon: ClipboardList },
      { title: "Profile", href: "/dashboard/profile", icon: FileText },
    ];

    // Add Team and Clients for Owner/Manager
    if ([UserRole.AgencyOwner, UserRole.AgencyManager].includes(user?.role as UserRole)) {
      quickActions.splice(1, 0, 
        { title: "Team", href: "/dashboard/agency/team", icon: Users }
      );
    }

    return (
      <div className="space-y-6">
        <PageHeader 
          title={getWelcomeMessage()} 
          description="Manage your agency and operations"
        />

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {quickActions.map((action) => {
            const Icon = action.icon;
            return (
              <Card key={action.href} className="hover:shadow-lg transition-shadow">
                <CardHeader>
                  <div className="flex items-center gap-2">
                    <Icon className="h-5 w-5 text-primary" />
                    <CardTitle className="text-lg">{action.title}</CardTitle>
                  </div>
                </CardHeader>
                <CardContent>
                  <Link href={action.href}>
                    <Button variant="outline" className="w-full">
                      Go to {action.title}
                    </Button>
                  </Link>
                </CardContent>
              </Card>
            );
          })}
        </div>
      </div>
    );
  }

  // For Studio members
  if ([UserRole.StudioOwner, UserRole.StudioManager, UserRole.StudioStaff].includes(user?.role as UserRole)) {
    const quickActions = [
      { title: "Studio Dashboard", href: "/dashboard/studio", icon: Building2 },
      { title: "Profile", href: "/dashboard/profile", icon: FileText },
    ];

    // Add Team for Owner/Manager
    if ([UserRole.StudioOwner, UserRole.StudioManager].includes(user?.role as UserRole)) {
      quickActions.splice(1, 0,
        { title: "Team", href: "/dashboard/studio/team", icon: Users }
      );
    }

    return (
      <div className="space-y-6">
        <PageHeader 
          title={getWelcomeMessage()} 
          description="Manage your studio and operations"
        />

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {quickActions.map((action) => {
            const Icon = action.icon;
            return (
              <Card key={action.href} className="hover:shadow-lg transition-shadow">
                <CardHeader>
                  <div className="flex items-center gap-2">
                    <Icon className="h-5 w-5 text-primary" />
                    <CardTitle className="text-lg">{action.title}</CardTitle>
                  </div>
                </CardHeader>
                <CardContent>
                  <Link href={action.href}>
                    <Button variant="outline" className="w-full">
                      Go to {action.title}
                    </Button>
                  </Link>
                </CardContent>
              </Card>
            );
          })}
        </div>
      </div>
    );
  }

  // Default fallback
  return (
    <div className="space-y-6">
      <PageHeader 
        title={getWelcomeMessage()} 
        description="Welcome to your dashboard"
      />
    </div>
  );
}
