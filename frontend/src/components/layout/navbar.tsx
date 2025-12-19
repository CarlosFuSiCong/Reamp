"use client";

import Link from "next/link";
import { useAuth } from "@/lib/hooks";
import { Button } from "@/components/ui/button";
import { Building2, LogOut, FileText, Bell, Home, Search, Sparkles, User, Settings, Menu } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Skeleton } from "@/components/ui/skeleton";
import { RoleBadge } from "@/components/shared/role-badge";
import { UserRole } from "@/types";
import { useState, useEffect } from "react";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";

interface NavbarProps {
  breadcrumbs?: { label: string; href?: string }[];
}

export function Navbar({ breadcrumbs }: NavbarProps) {
  const { user, isAuthenticated, isLoading, logout } = useAuth();
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 10);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  const getDashboardLink = () => {
    if (!isAuthenticated || !user) return "/";

    switch (user.role) {
      case UserRole.Admin:
        return "/dashboard/admin";
      case UserRole.Agent:
      case UserRole.Staff:
      case UserRole.User:
        return "/dashboard";
      default:
        return "/";
    }
  };

  const navLinks = [
    { href: "/", label: "Home", icon: Home },
    { href: "/listings", label: "Properties", icon: Search },
    { href: "/showcase", label: "Demo", icon: Sparkles },
  ];

  return (
    <nav 
      className={`sticky top-0 z-40 w-full transition-all duration-300 border-b ${
        scrolled 
          ? "bg-white/90 backdrop-blur-md shadow-sm border-gray-200/50" 
          : "bg-white border-transparent"
      }`}
    >
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between gap-4">
          {/* Logo */}
          <Link href="/" className="flex items-center gap-2 group transition-opacity hover:opacity-90">
            <div className="bg-blue-600 rounded-lg p-1.5 text-white shadow-lg shadow-blue-600/20 group-hover:shadow-blue-600/30 transition-shadow">
               <Building2 className="h-5 w-5" />
            </div>
            <span className="text-xl font-bold tracking-tight text-gray-900">Reamp</span>
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden md:flex items-center gap-1 absolute left-1/2 -translate-x-1/2">
            {navLinks.map((link) => (
              <Link key={link.href} href={link.href}>
                <Button variant="ghost" size="sm" className="gap-2 text-gray-600 hover:text-blue-600 hover:bg-blue-50/50 h-9 px-4 rounded-full font-medium transition-colors">
                  <link.icon className="h-4 w-4" />
                  {link.label}
              </Button>
            </Link>
            ))}
          </div>

          {/* Right Side Actions */}
          <div className="flex items-center gap-3">
            {isLoading ? (
              <Skeleton className="h-9 w-24 rounded-full" />
            ) : isAuthenticated && user ? (
              <>
                {/* Notification Bell (Desktop) */}
                <div className="hidden sm:block">
                {(user.role === UserRole.Admin ||
                  user.role === UserRole.Agent ||
                  user.role === UserRole.Staff) && (
                    <Button variant="ghost" size="icon" className="relative rounded-full hover:bg-gray-100 text-gray-500 hover:text-gray-900 w-9 h-9">
                    <Bell className="h-5 w-5" />
                      <span className="absolute top-2 right-2.5 h-2 w-2 rounded-full bg-red-500 ring-2 ring-white" />
                  </Button>
                )}
                </div>

                {/* User Dropdown */}
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" className="relative h-9 rounded-full pl-2 pr-4 gap-2 border border-gray-100 hover:border-gray-200 hover:bg-white bg-gray-50/50">
                      <Avatar className="h-7 w-7 border-2 border-white shadow-sm">
                        <AvatarFallback className="bg-gradient-to-br from-blue-500 to-blue-600 text-white text-xs">
                          {user.email?.charAt(0).toUpperCase() || "U"}
                        </AvatarFallback>
                      </Avatar>
                      <div className="hidden md:flex flex-col items-start gap-0.5">
                        <span className="text-xs font-semibold text-gray-700 leading-none max-w-[100px] truncate">
                           {user.email?.split('@')[0]}
                        </span>
                      </div>
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent className="w-56 mt-2" align="end" forceMount>
                    <DropdownMenuLabel className="font-normal p-3 bg-gray-50/50">
                      <div className="flex flex-col space-y-1">
                        <p className="text-sm font-medium leading-none text-gray-900">{user.email}</p>
                        <div className="flex items-center gap-2 pt-1">
                          <RoleBadge role={user.role} />
                        </div>
                      </div>
                    </DropdownMenuLabel>
                    <DropdownMenuSeparator />

                    <DropdownMenuItem asChild className="cursor-pointer">
                      <Link href={getDashboardLink()}>
                        <Building2 className="mr-2 h-4 w-4 text-gray-500" />
                        Dashboard
                      </Link>
                    </DropdownMenuItem>

                    <DropdownMenuItem asChild className="cursor-pointer">
                       <Link href="/dashboard/profile">
                        <User className="mr-2 h-4 w-4 text-gray-500" />
                        Profile
                       </Link>
                    </DropdownMenuItem>
                    
                    <DropdownMenuItem asChild className="cursor-pointer">
                       <Link href="/dashboard/settings">
                        <Settings className="mr-2 h-4 w-4 text-gray-500" />
                        Settings
                       </Link>
                    </DropdownMenuItem>

                    {user.role === UserRole.User && (
                      <DropdownMenuItem asChild className="cursor-pointer">
                        <Link href="/apply">
                          <FileText className="mr-2 h-4 w-4 text-gray-500" />
                          Apply for Organization
                        </Link>
                      </DropdownMenuItem>
                    )}

                    <DropdownMenuSeparator />
                    <DropdownMenuItem onClick={handleLogout} className="cursor-pointer text-red-600 focus:text-red-600 focus:bg-red-50">
                      <LogOut className="mr-2 h-4 w-4" />
                      Logout
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </>
            ) : (
              <div className="flex items-center gap-2">
                <Link href="/login" className="hidden sm:block">
                  <Button variant="ghost" size="sm" className="text-gray-600 hover:text-gray-900 hover:bg-gray-100">
                    Log in
                  </Button>
                </Link>
                <Link href="/register">
                  <Button size="sm" className="bg-blue-600 hover:bg-blue-700 text-white shadow-md shadow-blue-600/20 rounded-full px-5">
                     Get Started
                  </Button>
                </Link>
              </div>
            )}

            {/* Mobile Menu Toggle */}
            <div className="md:hidden ml-1">
               <Sheet>
                 <SheetTrigger asChild>
                   <Button variant="ghost" size="icon" className="h-9 w-9 text-gray-600">
                     <Menu className="h-5 w-5" />
                   </Button>
                 </SheetTrigger>
                 <SheetContent side="right" className="w-[300px] sm:w-[400px]">
                   <SheetHeader className="mb-6 text-left">
                     <SheetTitle className="flex items-center gap-2">
                        <div className="bg-blue-600 rounded-lg p-1.5 text-white">
                           <Building2 className="h-4 w-4" />
                        </div>
                        <span className="font-bold">Reamp</span>
                     </SheetTitle>
                   </SheetHeader>
                   <div className="flex flex-col gap-1">
                      {navLinks.map((link) => (
                        <Link key={link.href} href={link.href}>
                          <Button variant="ghost" className="w-full justify-start gap-3 h-11 text-base font-normal">
                             <link.icon className="h-5 w-5 text-gray-500" />
                             {link.label}
                          </Button>
                        </Link>
                      ))}
                      {!isAuthenticated && (
                         <>
                           <div className="h-px bg-gray-100 my-4" />
                           <Link href="/login">
                             <Button variant="ghost" className="w-full justify-start gap-3 h-11">
                                Log in
                             </Button>
                </Link>
              </>
            )}
                   </div>
                 </SheetContent>
               </Sheet>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}
