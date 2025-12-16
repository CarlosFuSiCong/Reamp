import { Activity } from "@/components/dashboard";
import { Listing, ShootOrder } from "@/types";

type ActivityWithSortKey = Activity & { sortKey: number };

export function generateRecentActivities(
  listings: Listing[],
  orders: ShootOrder[],
  maxItems: number = 5
): Activity[] {
  const now = Date.now();

  const activities: ActivityWithSortKey[] = [
    ...orders.slice(0, 3).map((order) => ({
      id: order.id,
      type: "order" as const,
      title: "New Order Created",
      description: `Order ${order.id.substring(0, 8)} has been created`,
      timestamp: new Date(order.createdAtUtc).toLocaleString(),
      sortKey: new Date(order.createdAtUtc).getTime(),
    })),
    ...listings.slice(0, 2).map((listing) => ({
      id: listing.id,
      type: "listing" as const,
      title: "Listing Updated",
      description: listing.title,
      timestamp: "Just now",
      sortKey: now,
    })),
  ];

  return activities
    .sort((a, b) => b.sortKey - a.sortKey)
    .slice(0, maxItems)
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    .map(({ sortKey, ...activity }) => activity);
}
