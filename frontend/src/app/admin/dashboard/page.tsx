"use client";

import { useQuery } from "@tanstack/react-query";
import { Users, Building2, ShoppingCart, Activity } from "lucide-react";
import { PageHeader } from "@/components/shared/page-header";
import { LoadingState } from "@/components/shared/loading-state";
import { ErrorState } from "@/components/shared/error-state";
import { StatsCard } from "@/components/dashboard";

export default function AdminDashboardPage() {
  return (
    <div>
      <PageHeader
        title="Admin Dashboard"
        description="System overview and management"
      />

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4 mb-8">
        <StatsCard
          title="Total Users"
          value={0}
          icon={Users}
          description="Registered users"
        />
        <StatsCard
          title="Total Listings"
          value={0}
          icon={Building2}
          description="All properties"
        />
        <StatsCard
          title="Total Orders"
          value={0}
          icon={ShoppingCart}
          description="All orders"
        />
        <StatsCard
          title="System Status"
          value="Active"
          icon={Activity}
          description="All systems operational"
        />
      </div>

      <div className="flex items-center justify-center h-64 border-2 border-dashed border-gray-300 rounded-lg">
        <p className="text-muted-foreground">Admin features coming soon...</p>
      </div>
    </div>
  );
}

