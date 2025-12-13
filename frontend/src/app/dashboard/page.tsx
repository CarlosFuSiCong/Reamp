"use client";

import { useAuth } from "@/lib/hooks";
import { PageHeader } from "@/components/shared";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { UserRole } from "@/types/enums";
import { Building2, Users, FileText } from "lucide-react";
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

  const getQuickActions = () => {
    if (!user) return [];

    const actions = [
      { title: "Profile", description: "Manage your profile", href: "/dashboard/profile", icon: FileText },
    ];

    switch (user.role) {
      case UserRole.AgencyOwner:
      case UserRole.AgencyManager:
        actions.push(
          { title: "Agency Team", description: "Manage team members", href: "/dashboard/agency/team", icon: Users },
          { title: "Clients", description: "Manage clients", href: "/dashboard/agency/clients", icon: Building2 }
        );
        break;
      
      case UserRole.StudioOwner:
      case UserRole.StudioManager:
        actions.push(
          { title: "Studio Team", description: "Manage team members", href: "/dashboard/studio/team", icon: Users }
        );
        break;
      
      case UserRole.Staff:
        actions.push(
          { title: "Apply", description: "Apply for organization", href: "/dashboard/profile/apply", icon: FileText }
        );
        break;
    }

    return actions;
  };

  const quickActions = getQuickActions();

  return (
    <div className="space-y-6">
      <PageHeader 
        title={getWelcomeMessage()} 
        description="Manage your account and organizations from here"
      />

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {quickActions.map((action) => {
          const Icon = action.icon;
          return (
            <Card key={action.href} className="hover:shadow-lg transition-shadow">
              <CardHeader>
                <div className="flex items-center gap-2">
                  <Icon className="h-5 w-5 text-primary" />
                  <CardTitle>{action.title}</CardTitle>
                </div>
                <CardDescription>{action.description}</CardDescription>
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

      {user?.role === UserRole.Staff && (
        <Card>
          <CardHeader>
            <CardTitle>Get Started</CardTitle>
            <CardDescription>
              You don't have an organization yet. Apply to create an Agency or Studio.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/dashboard/profile/apply">
              <Button>Apply for Organization</Button>
            </Link>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
