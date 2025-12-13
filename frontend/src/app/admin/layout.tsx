"use client";

import { ReactNode } from "react";
import { useAuth } from "@/lib/hooks/use-auth";
import { UserRole } from "@/types";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { LoadingState } from "@/components/shared/loading-state";
import { Sidebar, Header, SidebarNavItem } from "@/components/layout";
import { 
  LayoutDashboard, 
  Users, 
  Building2,
  Settings,
  FileText,
  ShieldCheck
} from "lucide-react";

const sidebarItems: SidebarNavItem[] = [
  { title: "Dashboard", href: "/admin", icon: LayoutDashboard },
  { title: "Users", href: "/admin/users", icon: Users },
  { title: "Agencies", href: "/admin/agencies", icon: Building2 },
  { title: "Studios", href: "/admin/studios", icon: Building2 },
  { title: "Settings", href: "/admin/settings", icon: Settings },
];

export default function AdminLayout({ children }: { children: ReactNode }) {
  const { user, isAuthenticated, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && (!user || user.role !== UserRole.Admin)) {
      router.push("/profile");
    }
  }, [user, isLoading, router]);

  if (isLoading) {
    return <LoadingState message="Checking permissions..." />;
  }

  if (!user || user.role !== UserRole.Admin) {
    return null;
  }

  return (
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
  );
}
