"use client";

import { ReactNode } from "react";
import { 
  LayoutDashboard, 
  Home, 
  ShoppingCart, 
  Users, 
  UserCircle, 
  Settings 
} from "lucide-react";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Sidebar, Header, SidebarNavItem } from "@/components/layout";

const sidebarItems: SidebarNavItem[] = [
  { title: "Dashboard", href: "/agent/dashboard", icon: LayoutDashboard },
  { title: "Listings", href: "/agent/listings", icon: Home },
  { title: "Orders", href: "/agent/orders", icon: ShoppingCart },
  { title: "Clients", href: "/agent/clients", icon: Users },
  { title: "Profile", href: "/profile", icon: UserCircle },
  { title: "Settings", href: "/agent/settings", icon: Settings },
];

export default function AgentLayout({ children }: { children: ReactNode }) {
  return (
    <ProtectedRoute allowedRoles={["Agent"]}>
      <div className="min-h-screen bg-gray-50">
        <div className="flex h-screen">
          <aside className="hidden w-64 border-r bg-background lg:block">
            <div className="flex h-16 items-center border-b px-6">
              <h1 className="text-xl font-bold">Reamp Agent</h1>
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

