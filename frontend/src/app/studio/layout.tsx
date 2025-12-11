"use client";

import { ReactNode } from "react";
import { 
  LayoutDashboard, 
  Camera,
  Calendar,
  FileImage,
  Settings,
  Users
} from "lucide-react";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Sidebar, Header, SidebarNavItem } from "@/components/layout";
import { UserRole } from "@/types";

const sidebarItems: SidebarNavItem[] = [
  { title: "Dashboard", href: "/studio/dashboard", icon: LayoutDashboard },
  { title: "Orders", href: "/studio/orders", icon: Camera },
  { title: "Schedule", href: "/studio/schedule", icon: Calendar },
  { title: "Gallery", href: "/studio/gallery", icon: FileImage },
  { title: "Clients", href: "/studio/clients", icon: Users },
  { title: "Settings", href: "/studio/settings", icon: Settings },
];

export default function StudioLayout({ children }: { children: ReactNode }) {
  return (
    <ProtectedRoute requiredRoles={[UserRole.Staff]}>
      <div className="min-h-screen bg-gray-50">
        <div className="flex h-screen">
          <aside className="hidden w-64 border-r bg-background lg:block">
            <div className="flex h-16 items-center border-b px-6">
              <h1 className="text-xl font-bold">Reamp Studio</h1>
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

