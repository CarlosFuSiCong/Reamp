"use client";

import { ReactNode } from "react";
import { 
  LayoutDashboard, 
  Users, 
  Building2,
  Settings,
  FileText,
  ShieldCheck
} from "lucide-react";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Sidebar, Header, SidebarNavItem } from "@/components/layout";
import { UserRole } from "@/types";

const sidebarItems: SidebarNavItem[] = [
  { title: "Dashboard", href: "/admin/dashboard", icon: LayoutDashboard },
  { title: "Users", href: "/admin/users", icon: Users },
  { title: "Listings", href: "/admin/listings", icon: Building2 },
  { title: "Orders", href: "/admin/orders", icon: FileText },
  { title: "Permissions", href: "/admin/permissions", icon: ShieldCheck },
  { title: "Settings", href: "/admin/settings", icon: Settings },
];

export default function AdminLayout({ children }: { children: ReactNode }) {
  return (
    <ProtectedRoute requiredRoles={[UserRole.Admin]}>
      <div className="min-h-screen bg-gray-50">
        <div className="flex h-screen">
          <aside className="hidden w-64 border-r bg-background lg:block">
            <div className="flex h-16 items-center border-b px-6">
              <h1 className="text-xl font-bold">Reamp Admin</h1>
            </div>
            <div className="p-4">
              <Sidebar items={sidebarItems} />
            </div>
          </aside>

          <div className="flex flex-1 flex-col overflow-hidden">
            <Header />
            <main className="flex-1 overflow-y-auto">
              <div className="container mx-auto py-6 px-4 md:px-6 lg:px-8">
                {children}
              </div>
            </main>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}

