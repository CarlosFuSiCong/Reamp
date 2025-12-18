import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { ShootOrder } from "@/types";
import { OrderCard } from "./order-card";
import { OrdersEmptyState } from "./orders-empty-state";
import { Users, Calendar, Upload, Clock } from "lucide-react";

interface StudioWorkflowTabsProps {
  needAssignment: ShootOrder[];
  readyToShoot: ShootOrder[];
  needUpload: ShootOrder[];
  awaitingConfirmation: ShootOrder[];
}

export function StudioWorkflowTabs({
  needAssignment,
  readyToShoot,
  needUpload,
  awaitingConfirmation,
}: StudioWorkflowTabsProps) {
  return (
    <Tabs defaultValue="assignment" className="space-y-6">
      <TabsList className="grid w-full grid-cols-4">
        <TabsTrigger value="assignment" className="flex items-center gap-2">
          <Users className="h-4 w-4" />
          <span className="hidden sm:inline">Assign Staff</span>
          <span className="sm:hidden">Assign</span>
          {needAssignment.length > 0 && (
            <Badge variant="secondary" className="ml-1">
              {needAssignment.length}
            </Badge>
          )}
        </TabsTrigger>
        <TabsTrigger value="shoot" className="flex items-center gap-2">
          <Calendar className="h-4 w-4" />
          <span className="hidden sm:inline">Ready to Shoot</span>
          <span className="sm:hidden">Shoot</span>
          {readyToShoot.length > 0 && (
            <Badge variant="secondary" className="ml-1">
              {readyToShoot.length}
            </Badge>
          )}
        </TabsTrigger>
        <TabsTrigger value="upload" className="flex items-center gap-2">
          <Upload className="h-4 w-4" />
          <span className="hidden sm:inline">Upload Delivery</span>
          <span className="sm:hidden">Upload</span>
          {needUpload.length > 0 && (
            <Badge variant="secondary" className="ml-1">
              {needUpload.length}
            </Badge>
          )}
        </TabsTrigger>
        <TabsTrigger value="pending" className="flex items-center gap-2">
          <Clock className="h-4 w-4" />
          <span className="hidden sm:inline">Pending</span>
          <span className="sm:hidden">Pending</span>
          {awaitingConfirmation.length > 0 && (
            <Badge variant="secondary" className="ml-1">
              {awaitingConfirmation.length}
            </Badge>
          )}
        </TabsTrigger>
      </TabsList>

      {/* Step 1: Assign Staff */}
      <TabsContent value="assignment" className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold">Assign Staff</h3>
            <p className="text-sm text-muted-foreground">
              Orders accepted, waiting for staff assignment
            </p>
          </div>
          <Badge variant="outline">{needAssignment.length} orders</Badge>
        </div>
        {needAssignment.length === 0 ? (
          <OrdersEmptyState
            icon={Users}
            title="No orders to assign"
            description="All accepted orders have been assigned to staff members."
          />
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {needAssignment.map((order) => (
              <OrderCard key={order.id} order={order} isStudio showActions />
            ))}
          </div>
        )}
      </TabsContent>

      {/* Step 2: Ready to Shoot */}
      <TabsContent value="shoot" className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold">Ready to Shoot</h3>
            <p className="text-sm text-muted-foreground">
              Staff assigned, ready to start shooting
            </p>
          </div>
          <Badge variant="outline">{readyToShoot.length} orders</Badge>
        </div>
        {readyToShoot.length === 0 ? (
          <OrdersEmptyState
            icon={Calendar}
            title="No scheduled shoots"
            description="No orders are currently scheduled for shooting."
          />
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {readyToShoot.map((order) => (
              <OrderCard key={order.id} order={order} isStudio />
            ))}
          </div>
        )}
      </TabsContent>

      {/* Step 3: Upload Delivery */}
      <TabsContent value="upload" className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold">Upload Delivery</h3>
            <p className="text-sm text-muted-foreground">
              Shooting completed, upload delivery package
            </p>
          </div>
          <Badge variant="outline">{needUpload.length} orders</Badge>
        </div>
        {needUpload.length === 0 ? (
          <OrdersEmptyState
            icon={Upload}
            title="No uploads pending"
            description="All completed shoots have been uploaded."
          />
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {needUpload.map((order) => (
              <OrderCard key={order.id} order={order} isStudio />
            ))}
          </div>
        )}
      </TabsContent>

      {/* Step 4: Awaiting Confirmation */}
      <TabsContent value="pending" className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold">Awaiting Confirmation</h3>
            <p className="text-sm text-muted-foreground">
              Delivery uploaded, waiting for agent confirmation
            </p>
          </div>
          <Badge variant="outline">{awaitingConfirmation.length} orders</Badge>
        </div>
        {awaitingConfirmation.length === 0 ? (
          <OrdersEmptyState
            icon={Clock}
            title="No pending confirmations"
            description="No deliveries are currently waiting for confirmation."
          />
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {awaitingConfirmation.map((order) => (
              <OrderCard key={order.id} order={order} isStudio />
            ))}
          </div>
        )}
      </TabsContent>
    </Tabs>
  );
}
