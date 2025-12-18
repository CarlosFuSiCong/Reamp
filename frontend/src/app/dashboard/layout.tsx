"use client";

import { ReactNode, useState } from "react";
import { useAuth, useProfile } from "@/lib/hooks";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Navbar } from "@/components/layout";
import { Sidebar, SidebarNavItem } from "@/components/layout/sidebar";
import { User, Users, ShoppingCart, Settings, ClipboardList, Building2, Package, LayoutDashboard, Menu } from "lucide-react";
import { UserRole, AgencyRole, StudioRole } from "@/types/enums";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";

export default function DashboardLayout({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const { user: profile, isLoading } = useProfile();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const getNavigationItems = (): SidebarNavItem[] => {
    if (!user || !profile) return [];

    // Base items for all authenticated users
    const baseItems: SidebarNavItem[] = [
      { title: "Overview", href: "/dashboard", icon: LayoutDashboard },
      { title: "Profile", href: "/dashboard/profile", icon: User },
    ];

    // Admin sees everything - check this FIRST before organization roles
    if (user.role === UserRole.Admin) {
      const adminItems: SidebarNavItem[] = [
        { title: "Overview", href: "/dashboard", icon: LayoutDashboard },
        { title: "Admin Panel", href: "/admin", icon: Settings },
        { title: "Profile", href: "/dashboard/profile", icon: User },
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

      return [...adminItems, ...orgItems];
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
      const roleValue =
        typeof profile.studioRole === "string"
        ? parseInt(profile.studioRole, 10)
        : Number(profile.studioRole);

      const studioItems: SidebarNavItem[] = [
        { title: "Studio", href: "/dashboard/studio", icon: Building2 },
        { title: "Marketplace", href: "/dashboard/marketplace", icon: ShoppingCart },
      ];

      // Only Owner (3) and Manager (2) can access staff assignment
      if (roleValue === 2 || roleValue === 3) {
        studioItems.push({ title: "Assign Staff", href: "/dashboard/staff-assignment", icon: Users });
      }

      studioItems.push(
        { title: "My Orders", href: "/dashboard/orders", icon: ClipboardList },
        { title: "Deliveries", href: "/dashboard/deliveries", icon: Package }
      );

      return [...baseItems, ...studioItems];
    }

    return baseItems;
  };

  const navItems = getNavigationItems();

  if (isLoading) {
    return (
      <ProtectedRoute>
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50/30 to-indigo-50/20 flex items-center justify-center">
          <div className="text-center space-y-4">
            <div className="relative w-16 h-16 mx-auto">
              <div className="absolute inset-0 rounded-full border-4 border-blue-200 animate-ping"></div>
              <div className="absolute inset-0 rounded-full border-4 border-t-blue-600 animate-spin"></div>
            </div>
            <p className="text-sm font-medium text-gray-600">Loading your workspace...</p>
          </div>
        </div>
      </ProtectedRoute>
    );
  }

  return (
    <ProtectedRoute>
      <div className="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50/30 to-indigo-50/20">
        <Navbar />
        
        {/* Mobile Menu Button */}
        <div className="md:hidden fixed bottom-6 right-6 z-50">
          <Sheet open={mobileMenuOpen} onOpenChange={setMobileMenuOpen}>
            <SheetTrigger asChild>
              <Button 
                size="lg" 
                className="h-14 w-14 rounded-full shadow-2xl bg-gradient-to-br from-blue-600 to-indigo-700 hover:from-blue-700 hover:to-indigo-800"
              >
                <Menu className="h-6 w-6" />
              </Button>
            </SheetTrigger>
            <SheetContent side="left" className="w-80 bg-white p-6">
              <SheetHeader className="mb-6">
                <SheetTitle className="text-left text-xl font-bold">Menu</SheetTitle>
              </SheetHeader>
              <div className="space-y-6">
                <Sidebar items={navItems} />
                
                {/* Help Section for Mobile */}
                <div className="bg-gradient-to-br from-blue-600 to-indigo-700 rounded-2xl p-6 text-white">
                  <div className="space-y-3">
                    <div className="h-10 w-10 rounded-xl bg-white/20 flex items-center justify-center">
                      <Settings className="h-5 w-5" />
                    </div>
                    <h3 className="font-semibold">Need Help?</h3>
                    <p className="text-sm text-blue-100">
                      Check out our documentation or contact support
                    </p>
                    <button 
                      className="w-full mt-2 px-4 py-2 bg-white text-blue-700 rounded-lg text-sm font-semibold hover:bg-blue-50 transition-colors"
                      onClick={() => setMobileMenuOpen(false)}
                    >
                      Get Support
                    </button>
                  </div>
                </div>
              </div>
            </SheetContent>
          </Sheet>
        </div>
        
        <div className="container mx-auto py-8 px-4 sm:px-6">
          <div className="grid grid-cols-12 gap-8">
            {/* Sidebar - Desktop Only */}
            <aside className="col-span-12 md:col-span-3 lg:col-span-2 hidden md:block">
              <div className="sticky top-24 space-y-6 animate-in fade-in slide-in-from-left-5 duration-700">
                {/* Menu Header */}
                <div className="px-4 py-3 rounded-xl bg-white shadow-sm border border-gray-100">
                  <div className="flex items-center gap-2">
                    <div className="h-2 w-2 rounded-full bg-green-500 animate-pulse"></div>
                    <p className="text-xs font-bold text-gray-500 uppercase tracking-wider">
                      Navigation
                    </p>
                  </div>
                </div>

                {/* Sidebar Navigation */}
                <div className="bg-white rounded-2xl shadow-lg border border-gray-100 p-3">
                  <Sidebar items={navItems} />
                </div>

                {/* Help Card */}
                <div className="bg-gradient-to-br from-blue-600 to-indigo-700 rounded-2xl p-6 text-white shadow-lg">
                  <div className="space-y-3">
                    <div className="h-10 w-10 rounded-xl bg-white/20 flex items-center justify-center">
                      <Settings className="h-5 w-5" />
                    </div>
                    <h3 className="font-semibold">Need Help?</h3>
                    <p className="text-sm text-blue-100">
                      Check out our documentation or contact support
                    </p>
                    <button className="w-full mt-2 px-4 py-2 bg-white text-blue-700 rounded-lg text-sm font-semibold hover:bg-blue-50 transition-colors">
                      Get Support
                    </button>
                  </div>
                </div>
              </div>
            </aside>

            {/* Main Content */}
            <main className="col-span-12 md:col-span-9 lg:col-span-10 min-h-[calc(100vh-8rem)] animate-in fade-in slide-in-from-bottom-5 duration-700 delay-100">
              {children}
            </main>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
