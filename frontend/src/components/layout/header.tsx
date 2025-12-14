"use client";

import Link from "next/link";
import { Bell, ChevronDown, LogOut, Building2 } from "lucide-react";
import { useAuthStore } from "@/lib/stores/auth-store";
import { useAuth } from "@/lib/hooks/use-auth";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { RoleBadge } from "@/components/shared/role-badge";
import { Button } from "@/components/ui/button";

interface HeaderProps {
  breadcrumbs?: { label: string; href?: string }[];
}

export function Header({ breadcrumbs }: HeaderProps) {
  const { user } = useAuthStore();
  const { logout } = useAuth();

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error("Logout error:", error);
    }
  };

  return (
    <header className="sticky top-0 z-30 flex h-16 items-center gap-4 border-b bg-background px-6">
      <Link href="/" className="flex items-center gap-2 mr-4">
        <Building2 className="h-5 w-5 text-primary" />
        <span className="font-semibold">Reamp</span>
      </Link>

      {breadcrumbs && breadcrumbs.length > 0 && (
        <nav className="flex items-center gap-2 text-sm text-muted-foreground">
          {breadcrumbs.map((crumb, index) => (
            <div key={index} className="flex items-center gap-2">
              {index > 0 && <span>/</span>}
              <span className={index === breadcrumbs.length - 1 ? "text-foreground font-medium" : ""}>
                {crumb.label}
              </span>
            </div>
          ))}
        </nav>
      )}

      <div className="ml-auto flex items-center gap-4">
        <Button variant="ghost" size="icon" className="relative">
          <Bell className="h-5 w-5" />
          <span className="absolute -right-1 -top-1 flex h-4 w-4 items-center justify-center rounded-full bg-destructive text-destructive-foreground text-xs">
            3
          </span>
        </Button>

        <div className="flex items-center gap-3">
          <Avatar className="h-8 w-8">
            <AvatarFallback>
              {user?.email?.substring(0, 2).toUpperCase() || "U"}
            </AvatarFallback>
          </Avatar>
          <div className="flex flex-col">
            <span className="text-sm font-medium">{user?.email}</span>
            {user?.role && <RoleBadge role={user.role} />}
          </div>
          <ChevronDown className="h-4 w-4 text-muted-foreground" />
        </div>

        <Button 
          variant="ghost" 
          size="sm" 
          onClick={handleLogout}
          className="gap-2"
        >
          <LogOut className="h-4 w-4" />
          Logout
        </Button>
      </div>
    </header>
  );
}


