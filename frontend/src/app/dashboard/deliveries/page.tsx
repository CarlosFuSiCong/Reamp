"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Plus, Package } from "lucide-react";
import { LoadingState } from "@/components/shared";
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
  const { data: ordersWithDeliveries, isLoading } = useQuery({
    queryKey: ["orders-with-deliveries", user?.studioId, user?.agencyId],
    queryFn: async () => {
      if (!user) return [];
      
      const ordersWithDeliveriesData: Array<{
        order: ShootOrder;
        deliveries: DeliveryPackageListDto[];
      }> = [];
      
      if (user.studioId) {
        const ordersResponse = await ordersApi.list({ 
          studioId: user.studioId, 
          page: 1, 
          pageSize: 100 
        });
        
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
        const ordersResponse = await ordersApi.list({ 
          agencyId: user.agencyId, 
          page: 1, 
          pageSize: 100 
        });
        
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
      {/* Header with Icon */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-orange-600 flex items-center justify-center text-white shadow-md">
              <Package className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">Deliveries</h1>
              <p className="text-sm text-gray-600 mt-1">
                {isStudio
                  ? "Manage delivery packages for completed orders"
                  : "View media deliveries from photography studios"}
              </p>
            </div>
          </div>
        </div>
        {isStudio && (
          <Button asChild className="shadow-md">
            <Link href="/dashboard/deliveries/new">
              <Plus className="mr-2 h-4 w-4" />
              Create Delivery
            </Link>
          </Button>
        )}
      </div>

      {/* Filters Card */}
      <Card className="shadow-md">
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
      <Card className="shadow-lg">
        <CardContent className="p-0">
          {!isLoading && !hasDeliveries ? (
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-16 w-16 rounded-full bg-orange-50 flex items-center justify-center mb-4">
                <Package className="h-8 w-8 text-orange-600" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">No deliveries yet</h3>
              <p className="text-sm text-gray-600 mb-6">
                {isStudio
                  ? "Create your first delivery package to share photos and videos with clients"
                  : "Deliveries from photography studios will appear here"}
              </p>
              {isStudio && (
                <Button asChild className="shadow-md">
                  <Link href="/dashboard/deliveries/new">
                    <Plus className="mr-2 h-4 w-4" />
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
