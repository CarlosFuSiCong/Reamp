"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Plus, Package } from "lucide-react";
import { PageHeader } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { deliveriesApi, ordersApi } from "@/lib/api";
import { useProfile } from "@/lib/hooks";
import { DeliveriesFilters } from "@/components/deliveries/deliveries-filters";
import { DeliveriesByOrderTable } from "@/components/deliveries/deliveries-by-order-table";
import Link from "next/link";
import type { DeliveryPackageListDto } from "@/types/delivery";
import type { ShootOrder } from "@/types";

export default function DeliveriesPage() {
  const { user } = useProfile();
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  // Fetch orders with their deliveries grouped together
  // TODO: Optimize with dedicated backend API to avoid N+1 queries
  const { data: ordersWithDeliveries, isLoading } = useQuery({
    queryKey: ["orders-with-deliveries", user?.studioId, user?.agencyId],
    queryFn: async () => {
      if (!user) return [];
      
      const ordersWithDeliveriesData: Array<{
        order: ShootOrder;
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
  const hasDeliveries = ordersWithDeliveries && ordersWithDeliveries.length > 0;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-orange-600 to-amber-700 flex items-center justify-center text-white">
              <Package className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">Deliveries</h1>
              <p className="text-muted-foreground mt-0.5">
                {isStudio
                  ? "Manage delivery packages for completed orders"
                  : "View media deliveries from photography studios"}
              </p>
            </div>
          </div>
        </div>
        {isStudio && (
          <Button asChild className="shadow-lg hover:shadow-xl transition-all">
            <Link href="/dashboard/deliveries/new">
              <Plus className="mr-2 h-4 w-4" />
              Create Delivery
            </Link>
          </Button>
        )}
      </div>

      {/* Filters Card */}
      <Card className="border-0 shadow-md">
        <CardContent className="pt-6">
          <DeliveriesFilters
            searchQuery={searchQuery}
            onSearchChange={setSearchQuery}
            statusFilter={statusFilter}
            onStatusChange={setStatusFilter}
          />
        </CardContent>
      </Card>

      {/* Deliveries Table Card */}
      <Card className="border-0 shadow-lg">
        <CardContent className="p-0">
          {!isLoading && !hasDeliveries ? (
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-24 w-24 rounded-full bg-gradient-to-br from-orange-100 to-amber-100 flex items-center justify-center mb-6">
                <Package className="h-12 w-12 text-orange-600" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">No deliveries yet</h3>
              <p className="text-gray-600 mb-6 max-w-md mx-auto">
                {isStudio
                  ? "Create your first delivery package to share photos and videos with clients"
                  : "Deliveries from photography studios will appear here"}
              </p>
              {isStudio && (
                <Button asChild size="lg" className="shadow-lg">
                  <Link href="/dashboard/deliveries/new">
                    <Plus className="mr-2 h-5 w-5" />
                    Create Your First Delivery
                  </Link>
                </Button>
              )}
            </div>
          ) : (
            <DeliveriesByOrderTable
              ordersWithDeliveries={ordersWithDeliveries || []}
              isLoading={isLoading}
              searchQuery={searchQuery}
              statusFilter={statusFilter}
            />
          )}
        </CardContent>
      </Card>
    </div>
  );
}
