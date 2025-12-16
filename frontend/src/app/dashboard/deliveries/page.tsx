"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { PageHeader } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { deliveriesApi, ordersApi } from "@/lib/api";
import { useProfile } from "@/lib/hooks";
import { DeliveriesTable } from "@/components/deliveries/deliveries-table";
import { DeliveriesFilters } from "@/components/deliveries/deliveries-filters";
import { DeliveriesByOrderTable } from "@/components/deliveries/deliveries-by-order-table";
import Link from "next/link";
import type { DeliveryPackageListDto } from "@/types/delivery";
import type { OrderDto } from "@/types";

export default function DeliveriesPage() {
  const { user } = useProfile();
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  // Fetch orders with their deliveries grouped together
  const { data: ordersWithDeliveries, isLoading } = useQuery({
    queryKey: ["orders-with-deliveries", user?.studioId, user?.agencyId],
    queryFn: async () => {
      if (!user) return [];
      
      const ordersWithDeliveriesData: Array<{
        order: OrderDto;
        deliveries: DeliveryPackageListDto[];
      }> = [];
      
      if (user.studioId) {
        // Fetch studio orders
        const ordersResponse = await ordersApi.list({ 
          studioId: user.studioId, 
          page: 1, 
          pageSize: 100 
        });
        
        // Fetch deliveries for each order
        for (const order of ordersResponse.items) {
          try {
            const orderDeliveries = await deliveriesApi.getByOrderId(order.id);
            if (orderDeliveries.length > 0) {
              ordersWithDeliveriesData.push({
                order,
                deliveries: orderDeliveries,
              });
            }
          } catch (error) {
            console.error(`Failed to fetch deliveries for order ${order.id}:`, error);
          }
        }
      } else if (user.agencyId) {
        // Fetch agency orders
        const ordersResponse = await ordersApi.list({ 
          agencyId: user.agencyId, 
          page: 1, 
          pageSize: 100 
        });
        
        // Fetch deliveries for each order
        for (const order of ordersResponse.items) {
          try {
            const orderDeliveries = await deliveriesApi.getByOrderId(order.id);
            if (orderDeliveries.length > 0) {
              ordersWithDeliveriesData.push({
                order,
                deliveries: orderDeliveries,
              });
            }
          } catch (error) {
            console.error(`Failed to fetch deliveries for order ${order.id}:`, error);
          }
        }
      }
      
      return ordersWithDeliveriesData;
    },
    enabled: !!user,
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

      <DeliveriesByOrderTable
        ordersWithDeliveries={ordersWithDeliveries || []}
        isLoading={isLoading}
        searchQuery={searchQuery}
        statusFilter={statusFilter}
      />
    </div>
  );
}
