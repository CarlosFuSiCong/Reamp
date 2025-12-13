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
  FileText, 
  Building2,
  Briefcase,
  ShoppingCart,
  Settings,
  ClipboardList
} from "lucide-react";
import { UserRole } from "@/types/enums";

export default function DashboardLayout({ children }: { children: ReactNode }) {
  const { user } = useAuth();

  // Base navigation items for all users
  const baseNavItems: SidebarNavItem[] = [
    { title: "Dashboard", href: "/dashboard", icon: LayoutDashboard },
    { title: "Profile", href: "/dashboard/profile", icon: User },
  ];

  // Agency-specific navigation
  const agencyNavItems: SidebarNavItem[] = [
    { title: "Agency Dashboard", href: "/dashboard/agency", icon: Building2 },
    { title: "Team", href: "/dashboard/agency/team", icon: Users },
    { title: "Clients", href: "/dashboard/agency/clients", icon: Briefcase },
    { title: "Listings", href: "/dashboard/agency/listings", icon: ClipboardList },
    { title: "Orders", href: "/dashboard/agency/orders", icon: ShoppingCart },
    { title: "Settings", href: "/dashboard/agency/settings", icon: Settings },
  ];

  // Studio-specific navigation
  const studioNavItems: SidebarNavItem[] = [
    { title: "Studio Dashboard", href: "/dashboard/studio", icon: Building2 },
    { title: "Team", href: "/dashboard/studio/team", icon: Users },
  ];

  // Admin-specific navigation
  const adminNavItems: SidebarNavItem[] = [
    { title: "Admin Panel", href: "/admin", icon: Settings },
  ];

  // Apply navigation
  const applyNavItems: SidebarNavItem[] = [
    { title: "Apply for Organization", href: "/dashboard/profile/apply", icon: FileText },
  ];

  // Build navigation based on user role
  const getNavigationItems = (): SidebarNavItem[] => {
    const items = [...baseNavItems];

    if (!user) return items;

    // Add role-specific items
    switch (user.role) {
      case UserRole.AgencyOwner:
      case UserRole.AgencyManager:
      case UserRole.AgencyStaff:
        items.push(...agencyNavItems);
        break;
      
      case UserRole.StudioOwner:
      case UserRole.StudioManager:
      case UserRole.StudioStaff:
        items.push(...studioNavItems);
        break;
      
      case UserRole.Admin:
        items.push(...adminNavItems);
        items.push(...agencyNavItems);
        items.push(...studioNavItems);
        break;
      
      case UserRole.Staff:
        // Staff without organization can apply
        items.push(...applyNavItems);
        break;
    }

    return items;
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
