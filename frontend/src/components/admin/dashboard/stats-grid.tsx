import { StatsCard } from "@/components/dashboard/stats-card";
import { Users, Package, ShoppingCart, Building2 } from "lucide-react";

interface StatsGridProps {
  stats: {
    totalUsers: number;
    activeListings: number;
    totalOrders: number;
    totalStudios: number;
  };
}

export function StatsGrid({ stats }: StatsGridProps) {
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <StatsCard
        title="Total Users"
        value={stats.totalUsers}
        icon={Users}
        description="Active platform users"
      />
      <StatsCard
        title="Active Listings"
        value={stats.activeListings}
        icon={Package}
        description="Currently active"
      />
      <StatsCard
        title="Total Orders"
        value={stats.totalOrders}
        icon={ShoppingCart}
        description="All time orders"
      />
      <StatsCard
        title="Studios"
        value={stats.totalStudios}
        icon={Building2}
        description="Registered studios"
      />
    </div>
  );
}
