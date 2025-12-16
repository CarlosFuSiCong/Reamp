"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { PageHeader } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { deliveriesApi } from "@/lib/api";
import { useProfile } from "@/lib/hooks";
import { DeliveriesTable } from "@/components/deliveries/deliveries-table";
import { DeliveriesFilters } from "@/components/deliveries/deliveries-filters";
import Link from "next/link";

export default function DeliveriesPage() {
  const { user } = useProfile();
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  // For now, we'll fetch all deliveries
  // TODO: Add proper filtering and pagination
  const { data: deliveries, isLoading } = useQuery({
    queryKey: ["deliveries", "all"],
    queryFn: async () => {
      // This will need to be updated based on user role
      // For now, return empty array
      return [];
    },
  });

  const isStudio = !!user?.studioId;

  return (
    <div className="space-y-6">
      <PageHeader
        title="Deliveries"
        description={
          isStudio
            ? "Manage delivery packages for completed orders"
            : "View media deliveries from photography studios"
        }
        action={
          isStudio ? (
            <Link href="/dashboard/deliveries/new">
              <Button>
                <Plus className="mr-2 h-4 w-4" />
                Create Delivery
              </Button>
            </Link>
          ) : undefined
        }
      />

      <DeliveriesFilters
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
        statusFilter={statusFilter}
        onStatusChange={setStatusFilter}
      />

      <DeliveriesTable
        deliveries={deliveries || []}
        isLoading={isLoading}
        searchQuery={searchQuery}
        statusFilter={statusFilter}
      />
    </div>
  );
}
