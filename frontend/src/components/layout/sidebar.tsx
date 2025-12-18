"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils";
import { LucideIcon, ChevronRight } from "lucide-react";

export interface SidebarNavItem {
  title: string;
  href: string;
  icon: LucideIcon;
}

interface SidebarProps {
  items: SidebarNavItem[];
  className?: string;
}

export function Sidebar({ items, className }: SidebarProps) {
  const pathname = usePathname();

  return (
    <nav className={cn("space-y-2", className)}>
      {items.map((item, index) => {
        const Icon = item.icon;
        // Active if exact match OR if pathname starts with href (for child pages)
        const isActive = pathname === item.href || (item.href !== "/dashboard" && pathname.startsWith(`${item.href}/`));

        return (
          <Link
            key={item.href}
            href={item.href}
            className={cn(
              "group relative flex items-center gap-3 rounded-xl px-4 py-3 text-sm font-medium transition-all duration-200 ease-in-out",
              "hover:translate-x-1",
              isActive
                ? "bg-gradient-to-r from-blue-600 to-blue-700 text-white shadow-lg shadow-blue-600/30"
                : "text-gray-700 hover:bg-gray-100 hover:text-gray-900"
            )}
            style={{
              animationDelay: `${index * 50}ms`,
            }}
          >
            {/* Active indicator bar */}
            {isActive && (
              <div className="absolute left-0 top-1/2 -translate-y-1/2 w-1 h-8 bg-white rounded-r-full" />
            )}

            {/* Icon with gradient background when active */}
            <div className={cn(
              "relative flex items-center justify-center rounded-lg p-1.5 transition-all",
              isActive 
                ? "bg-white/20" 
                : "bg-transparent group-hover:bg-gray-200"
            )}>
              <Icon className={cn(
                "h-4 w-4 transition-all",
                isActive ? "text-white" : "text-gray-500 group-hover:text-gray-700"
              )} />
            </div>

            {/* Title */}
            <span className="flex-1">{item.title}</span>

            {/* Arrow indicator */}
            <ChevronRight className={cn(
              "h-4 w-4 transition-all",
              isActive 
                ? "opacity-100 translate-x-0 text-white" 
                : "opacity-0 -translate-x-2 text-gray-400 group-hover:opacity-100 group-hover:translate-x-0"
            )} />

            {/* Shine effect on hover */}
            {!isActive && (
              <div className="absolute inset-0 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity duration-300 pointer-events-none bg-gradient-to-r from-transparent via-white/5 to-transparent" />
            )}
          </Link>
        );
      })}
    </nav>
  );
}
