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
    <Card className="group hover:shadow-xl transition-all duration-300 flex flex-col border-2 hover:border-blue-200 overflow-hidden">
      {/* Status Indicator Bar */}
      <div className={`h-1.5 w-full ${
        order.status === OrderStatus.Completed ? 'bg-gradient-to-r from-green-500 to-emerald-500' :
        order.status === OrderStatus.InProgress ? 'bg-gradient-to-r from-blue-500 to-cyan-500' :
        order.status === OrderStatus.Accepted ? 'bg-gradient-to-r from-purple-500 to-pink-500' :
        order.status === OrderStatus.Placed ? 'bg-gradient-to-r from-orange-500 to-yellow-500' :
        'bg-gradient-to-r from-gray-400 to-gray-500'
      }`} />

      <Link href={`/dashboard/orders/${order.id}`} className="flex-1">
        <CardHeader className="pb-4 space-y-3">
          <div className="flex items-start justify-between gap-3">
            <div className="flex-1 space-y-3">
              {/* Status and Date Row */}
              <div className="flex items-center gap-2 flex-wrap">
                <Badge 
                  variant={statusConfig.variant as any}
                  className="font-semibold shadow-sm"
                >
                  {statusConfig.label}
                </Badge>
                {order.scheduledStartUtc && (
                  <div className="flex items-center gap-1.5 text-xs text-gray-600 bg-gray-50 px-2.5 py-1 rounded-full">
                    <Calendar className="h-3.5 w-3.5" />
                    <span className="font-medium">
                      {new Date(order.scheduledStartUtc).toLocaleDateString('en-US', { 
                        month: 'short', 
                        day: 'numeric',
                        year: 'numeric'
                      })}
                    </span>
                  </div>
                )}
              </div>

              {/* Title */}
              <h3 className="font-bold text-lg text-gray-900 group-hover:text-blue-600 transition-colors line-clamp-2">
                {order.listingTitle || "Untitled Listing"}
              </h3>

              {/* Address */}
              {order.listingAddress && (
                <div className="flex items-start gap-2 text-sm text-gray-600">
                  <MapPin className="h-4 w-4 mt-0.5 flex-shrink-0 text-gray-400" />
                  <span className="line-clamp-2">{order.listingAddress}</span>
                </div>
              )}
            </div>

            {/* Arrow Icon */}
            <div className="h-8 w-8 rounded-full bg-gray-100 group-hover:bg-blue-100 flex items-center justify-center flex-shrink-0 transition-colors">
              <ChevronRight className="h-5 w-5 text-gray-400 group-hover:text-blue-600 transition-colors" />
            </div>
          </div>
        </CardHeader>

        <CardContent className="space-y-4">
          {/* Info Grid */}
          <div className="grid grid-cols-2 gap-3">
            {/* Studio/Agency Info */}
            {isStudio && order.agencyName && (
              <div className="col-span-2 flex items-center gap-3 p-3 bg-purple-50 rounded-lg border border-purple-100">
                <div className="h-10 w-10 rounded-lg bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center flex-shrink-0 shadow-sm">
                  <Building2 className="h-5 w-5 text-white" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-medium text-purple-700 uppercase tracking-wide">Agency</p>
                  <p className="font-semibold text-gray-900 truncate">{order.agencyName}</p>
                </div>
              </div>
            )}
            {isAgent && order.studioName && (
              <div className="col-span-2 flex items-center gap-3 p-3 bg-blue-50 rounded-lg border border-blue-100">
                <div className="h-10 w-10 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center flex-shrink-0 shadow-sm">
                  <Camera className="h-5 w-5 text-white" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-xs font-medium text-blue-700 uppercase tracking-wide">Studio</p>
                  <p className="font-semibold text-gray-900 truncate">{order.studioName}</p>
                </div>
              </div>
            )}

            {/* Amount */}
            <div className="flex items-center gap-3 p-3 bg-green-50 rounded-lg border border-green-100">
              <div className="h-9 w-9 rounded-lg bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center flex-shrink-0 shadow-sm">
                <DollarSign className="h-5 w-5 text-white" />
              </div>
              <div>
                <p className="text-xs font-medium text-green-700 uppercase tracking-wide">Amount</p>
                <p className="font-bold text-gray-900">
                  {order.currency} ${order.totalAmount.toFixed(2)}
                </p>
              </div>
            </div>

            {/* Tasks */}
            {order.taskCount !== undefined && order.taskCount > 0 && (
              <div className="flex items-center gap-3 p-3 bg-orange-50 rounded-lg border border-orange-100">
                <div className="h-9 w-9 rounded-lg bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center flex-shrink-0 shadow-sm">
                  <CheckCircle2 className="h-5 w-5 text-white" />
                </div>
                <div>
                  <p className="text-xs font-medium text-orange-700 uppercase tracking-wide">Tasks</p>
                  <p className="font-bold text-gray-900">{order.taskCount}</p>
                </div>
              </div>
            )}
          </div>

          {/* Created Time */}
          <div className="flex items-center justify-between pt-2 border-t border-gray-100">
            <span className="text-xs text-gray-500">Created</span>
            <span className="text-sm font-medium text-gray-700">
              {formatDistanceToNow(new Date(order.createdAtUtc), { addSuffix: true })}
            </span>
          </div>
        </CardContent>
      </Link>
      
      {/* Action Button */}
      {showActions && needsStaffAssignment && (
        <CardFooter className="pt-0 pb-4 px-6">
          <Button asChild className="w-full shadow-md hover:shadow-lg transition-shadow" size="sm">
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
