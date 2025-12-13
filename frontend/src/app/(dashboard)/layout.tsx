"use client";

import { ReactNode } from "react";
import { ProtectedRoute } from "@/components/auth/protected-route";
import { Navbar } from "@/components/layout";

export default function DashboardLayout({ children }: { children: ReactNode }) {
  return (
    <ProtectedRoute>
      <div className="min-h-screen bg-gray-50">
        <Navbar />
        <div className="container mx-auto py-8 px-4">{children}</div>
      </div>
    </ProtectedRoute>
  );
}

