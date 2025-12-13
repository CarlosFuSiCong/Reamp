"use client";

import { ReactNode } from "react";
import { useAuth } from "@/lib/hooks";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Navbar } from "@/components/layout";
import { Sidebar, SidebarNavItem } from "@/components/layout/sidebar";
import { 
  LayoutDashboard, 
  User, 
  Users, 
  Briefcase,
  ShoppingCart,
  Settings,
  ClipboardList,
  Building2
} from "lucide-react";
import { UserRole } from "@/types/enums";

export default function DashboardLayout({ children }: { children: ReactNode }) {
  const { user } = useAuth();

  const getNavigationItems = (): SidebarNavItem[] => {
    if (!user) return [];

    // Base items for all authenticated users
    const baseItems: SidebarNavItem[] = [
      { title: "Dashboard", href: "/dashboard", icon: LayoutDashboard },
      { title: "Profile", href: "/dashboard/profile", icon: User },
    ];

    // For regular Staff (no organization)
    if (user.role === UserRole.Staff) {
      return baseItems;
    }

    // Agency navigation
    if ([UserRole.AgencyOwner, UserRole.AgencyManager, UserRole.AgencyStaff].includes(user.role)) {
      const agencyItems: SidebarNavItem[] = [
        { title: "Agency Dashboard", href: "/dashboard/agency", icon: Building2 },
      ];

      // Owner & Manager can manage team and clients
      if ([UserRole.AgencyOwner, UserRole.AgencyManager].includes(user.role)) {
        agencyItems.push(
          { title: "Team", href: "/dashboard/agency/team", icon: Users },
          { title: "Clients", href: "/dashboard/agency/clients", icon: Briefcase }
        );
      }

      // All agency members can view listings and orders
      agencyItems.push(
        { title: "Listings", href: "/dashboard/agency/listings", icon: ClipboardList },
        { title: "Orders", href: "/dashboard/agency/orders", icon: ShoppingCart }
      );

      // Only Owner can access settings
      if (user.role === UserRole.AgencyOwner) {
        agencyItems.push({ title: "Settings", href: "/dashboard/agency/settings", icon: Settings });
      }

      return [...baseItems, ...agencyItems];
    }

    // Studio navigation
    if ([UserRole.StudioOwner, UserRole.StudioManager, UserRole.StudioStaff].includes(user.role)) {
      const studioItems: SidebarNavItem[] = [
        { title: "Studio Dashboard", href: "/dashboard/studio", icon: Building2 },
      ];

      // Owner & Manager can manage team
      if ([UserRole.StudioOwner, UserRole.StudioManager].includes(user.role)) {
        studioItems.push({ title: "Team", href: "/dashboard/studio/team", icon: Users });
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
