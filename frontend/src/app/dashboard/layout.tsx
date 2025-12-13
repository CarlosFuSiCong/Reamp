"use client";

import { ReactNode } from "react";
import { useAuth, useProfile } from "@/lib/hooks";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Navbar } from "@/components/layout";
import { Sidebar, SidebarNavItem } from "@/components/layout/sidebar";
import { 
  User, 
  Users, 
  Briefcase,
  ShoppingCart,
  Settings,
  ClipboardList,
  Building2
} from "lucide-react";
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

    // For regular Staff (no organization)
    if (user.role === UserRole.Staff && !profile.agencyRole && !profile.studioRole) {
      return baseItems;
    }

    // Agency navigation (user has agencyRole)
    if (profile.agencyRole !== undefined && profile.agencyRole !== null) {
      const agencyItems: SidebarNavItem[] = [
        { title: "Agency", href: "/dashboard/agency", icon: Building2 },
      ];

      const roleValue = typeof profile.agencyRole === 'string' 
        ? parseInt(profile.agencyRole, 10) 
        : Number(profile.agencyRole);

      // Owner & Manager can manage team and clients
      if (roleValue === AgencyRole.Owner || roleValue === AgencyRole.Manager) {
        agencyItems.push(
          { title: "Team", href: "/dashboard/team", icon: Users },
          { title: "Clients", href: "/dashboard/agency/clients", icon: Briefcase }
        );
      }

      // All agency members can view listings and orders
      agencyItems.push(
        { title: "Listings", href: "/dashboard/agency/listings", icon: ClipboardList },
        { title: "Orders", href: "/dashboard/agency/orders", icon: ShoppingCart }
      );

      // Only Owner can access settings
      if (roleValue === AgencyRole.Owner) {
        agencyItems.push({ title: "Settings", href: "/dashboard/agency/settings", icon: Settings });
      }

      return [...baseItems, ...agencyItems];
    }

    // Studio navigation (user has studioRole)
    if (profile.studioRole !== undefined && profile.studioRole !== null) {
      const studioItems: SidebarNavItem[] = [
        { title: "Studio", href: "/dashboard/studio", icon: Building2 },
      ];

      const roleValue = typeof profile.studioRole === 'string' 
        ? parseInt(profile.studioRole, 10) 
        : Number(profile.studioRole);

      // Owner & Manager can manage team
      if (roleValue === StudioRole.Owner || roleValue === StudioRole.Manager) {
        studioItems.push({ title: "Team", href: "/dashboard/team", icon: Users });
      }

      // All studio members can view orders (studio side)
      // Add more studio-specific items here as needed

      return [...baseItems, ...studioItems];
    }

    // Admin sees everything (optional)
    if (user.role === UserRole.Admin) {
      return [
        ...baseItems,
        { title: "Admin Panel", href: "/admin", icon: Settings },
      ];
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
            <main className="col-span-12 md:col-span-9 lg:col-span-10">
              {children}
            </main>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
