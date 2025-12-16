"use client";

import { ReactNode } from "react";
import { useAuth, useProfile } from "@/lib/hooks";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Navbar } from "@/components/layout";
import { Sidebar, SidebarNavItem } from "@/components/layout/sidebar";
import { User, Users, ShoppingCart, Settings, ClipboardList, Building2, Package } from "lucide-react";
import { UserRole, AgencyRole, StudioRole } from "@/types/enums";

export default function DashboardLayout({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const { user: profile, isLoading } = useProfile();

  const getNavigationItems = (): SidebarNavItem[] => {
    if (!user || !profile) return [];

    // Base items for all authenticated users
    const baseItems: SidebarNavItem[] = [
      { title: "Profile", href: "/dashboard/profile", icon: User },
    ];

    // Admin sees everything - check this FIRST before organization roles
    if (user.role === UserRole.Admin) {
      const adminItems: SidebarNavItem[] = [
        { title: "Admin Panel", href: "/admin", icon: Settings },
      ];

      // Admins might also have organization roles, so include those too
      const orgItems: SidebarNavItem[] = [];

      // Add agency navigation if admin is also an agency member
      if (profile.agencyRole !== undefined && profile.agencyRole !== null) {
        orgItems.push({ title: "Agency", href: "/dashboard/agency", icon: Building2 });
      }

      // Add studio navigation if admin is also a studio member
      if (profile.studioRole !== undefined && profile.studioRole !== null) {
        orgItems.push({ title: "Studio", href: "/dashboard/studio", icon: Building2 });
      }

      return [...baseItems, ...adminItems, ...orgItems];
    }

    // Agency navigation (user has agencyRole)
    if (profile.agencyRole !== undefined && profile.agencyRole !== null) {
      const agencyItems: SidebarNavItem[] = [
        { title: "Agency", href: "/dashboard/agency", icon: Building2 },
      ];

      const roleValue =
        typeof profile.agencyRole === "string"
          ? parseInt(profile.agencyRole, 10)
          : Number(profile.agencyRole);

      // Owner & Manager can manage team
      if (roleValue === AgencyRole.Owner || roleValue === AgencyRole.Manager) {
        agencyItems.push({ title: "Team", href: "/dashboard/team", icon: Users });
      }

      // All agency members can view listings, orders, and deliveries
      agencyItems.push(
        { title: "Listings", href: "/dashboard/listings", icon: ClipboardList },
        { title: "Orders", href: "/dashboard/orders", icon: ShoppingCart },
        { title: "Deliveries", href: "/dashboard/deliveries", icon: Package }
      );

      return [...baseItems, ...agencyItems];
    }

    // Studio navigation (user has studioRole)
    if (profile.studioRole !== undefined && profile.studioRole !== null) {
      const studioItems: SidebarNavItem[] = [
        { title: "Studio", href: "/dashboard/studio", icon: Building2 },
        { title: "Marketplace", href: "/dashboard/marketplace", icon: ShoppingCart },
        { title: "My Orders", href: "/dashboard/orders", icon: ClipboardList },
        { title: "Deliveries", href: "/dashboard/deliveries", icon: Package },
      ];

      return [...baseItems, ...studioItems];
    }

    return baseItems;
  };

  const navItems = getNavigationItems();

  if (isLoading) {
    return (
      <ProtectedRoute>
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        </div>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute>
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="container mx-auto py-6 px-4">
          <div className="grid grid-cols-12 gap-6">
            {/* Sidebar */}
            <aside className="col-span-12 md:col-span-3 lg:col-span-2">
              <div className="sticky top-6">
                <Sidebar items={navItems} />
              </div>
            </aside>

            {/* Main Content */}
            <main className="col-span-12 md:col-span-9 lg:col-span-10">{children}</main>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
