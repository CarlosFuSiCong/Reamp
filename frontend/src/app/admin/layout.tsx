"use client";

import { ReactNode } from "react";
import { useAuth } from "@/lib/hooks/use-auth";
import { UserRole } from "@/types";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { LoadingState } from "@/components/shared/loading-state";
import { Sidebar, Navbar, SidebarNavItem } from "@/components/layout";
import { LayoutDashboard, User } from "lucide-react";

const sidebarItems: SidebarNavItem[] = [
  { title: "Admin Panel", href: "/admin", icon: LayoutDashboard },
  { title: "Profile", href: "/dashboard/profile", icon: User },
];

export default function AdminLayout({ children }: { children: ReactNode }) {
  const { user, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && (!user || user.role !== UserRole.Admin)) {
      router.push("/");
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
      <Navbar breadcrumbs={[{ label: "Admin" }]} />
      
      <div className="container mx-auto py-6 px-4">
        <div className="grid grid-cols-12 gap-6">
          {/* Sidebar */}
          <aside className="col-span-12 md:col-span-3 lg:col-span-2">
            <div className="sticky top-6">
              <Sidebar items={sidebarItems} />
            </div>
          </aside>

          {/* Main Content */}
          <main className="col-span-12 md:col-span-9 lg:col-span-10">{children}</main>
        </div>
      </div>
    </div>
  );
}
