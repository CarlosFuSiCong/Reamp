import Link from "next/link";
import { Card, CardContent, CardHeader, CardFooter } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { ShootOrder, OrderStatus } from "@/types";
import { getOrderStatusConfig } from "@/lib/utils/enum-labels";
import { Calendar, MapPin, Building2, Camera, ChevronRight, Users, DollarSign, CheckCircle2 } from "lucide-react";
import { formatDistanceToNow } from "date-fns";

interface OrderCardProps {
  order: ShootOrder;
  isAgent?: boolean;
  isStudio?: boolean;
  showActions?: boolean;
}

export function OrderCard({ order, isAgent, isStudio, showActions = false }: OrderCardProps) {
  const statusConfig = getOrderStatusConfig(order.status);
  const needsStaffAssignment = isStudio && order.status === OrderStatus.Accepted;

  return (
    <Card className="group hover:shadow-lg transition-shadow flex flex-col">
      <Link href={`/dashboard/orders/${order.id}`} className="flex-1">
        <CardHeader className="pb-4">
          <div className="flex items-start justify-between gap-3">
            <div className="flex-1 space-y-3">
              {/* Status and Date */}
              <div className="flex items-center gap-2 flex-wrap">
                <Badge variant={statusConfig.variant as any}>
                  {statusConfig.label}
                </Badge>
                {order.scheduledStartUtc && (
                  <span className="text-xs text-gray-500 flex items-center gap-1">
                    <Calendar className="h-3.5 w-3.5" />
                    {new Date(order.scheduledStartUtc).toLocaleDateString('en-US', { 
                      month: 'short', 
                      day: 'numeric',
                      year: 'numeric'
                    })}
                  </span>
                )}
              </div>

              {/* Title */}
              <h3 className="font-semibold text-lg text-gray-900 line-clamp-2">
                {order.listingTitle || "Untitled Listing"}
              </h3>

              {/* Address */}
              {order.listingAddress && (
                <div className="flex items-start gap-2 text-sm text-gray-600">
                  <MapPin className="h-4 w-4 mt-0.5 flex-shrink-0" />
                  <span className="line-clamp-2">{order.listingAddress}</span>
                </div>
              )}
            </div>

            {/* Arrow */}
            <ChevronRight className="h-5 w-5 text-gray-400 flex-shrink-0 mt-1" />
          </div>
        </CardHeader>

        <CardContent className="space-y-3">
          {/* Studio/Agency */}
          {isStudio && order.agencyName && (
            <div className="flex items-center gap-2 pb-3 border-b">
              <Building2 className="h-4 w-4 text-gray-400" />
              <div>
                <p className="text-xs text-gray-500">Agency</p>
                <p className="font-medium text-gray-900">{order.agencyName}</p>
              </div>
            </div>
          )}
          {isAgent && order.studioName && (
            <div className="flex items-center gap-2 pb-3 border-b">
              <Camera className="h-4 w-4 text-gray-400" />
              <div>
                <p className="text-xs text-gray-500">Studio</p>
                <p className="font-medium text-gray-900">{order.studioName}</p>
              </div>
            </div>
          )}

          {/* Info Grid */}
          <div className="grid grid-cols-2 gap-x-4 gap-y-3 text-sm">
            {/* Amount */}
            <div className="flex items-center gap-2">
              <DollarSign className="h-4 w-4 text-green-600" />
              <div>
                <p className="text-xs text-gray-500">Amount</p>
                <p className="font-semibold text-gray-900">
                  {order.currency} ${order.totalAmount.toFixed(2)}
                </p>
              </div>
            </div>

            {/* Tasks */}
            {order.taskCount !== undefined && order.taskCount > 0 && (
              <div className="flex items-center gap-2">
                <CheckCircle2 className="h-4 w-4 text-blue-600" />
                <div>
                  <p className="text-xs text-gray-500">Tasks</p>
                  <p className="font-semibold text-gray-900">{order.taskCount}</p>
                </div>
              </div>
            )}

            {/* Created */}
            <div className="col-span-2 flex items-center justify-between pt-2 border-t text-xs">
              <span className="text-gray-500">Created</span>
              <span className="font-medium text-gray-700">
                {formatDistanceToNow(new Date(order.createdAtUtc), { addSuffix: true })}
              </span>
            </div>
          </div>
        </CardContent>
      </Link>
      
      {/* Action Button */}
      {showActions && needsStaffAssignment && (
        <CardFooter className="pt-0 pb-4 px-6">
          <Button asChild className="w-full" size="sm">
            <Link href={`/dashboard/orders/${order.id}/assign`}>
              <Users className="mr-2 h-4 w-4" />
              Assign Staff
            </Link>
          </Button>
        </CardFooter>
      )}
    </Card>
  );
}
