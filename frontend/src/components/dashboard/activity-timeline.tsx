import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { cn } from "@/lib/utils";

export interface Activity {
  id: string;
  type: "order" | "listing" | "message" | "other";
  title: string;
  description: string;
  timestamp: string;
}

interface ActivityTimelineProps {
  activities: Activity[];
  className?: string;
}

export function ActivityTimeline({ activities, className }: ActivityTimelineProps) {
  const getActivityColor = (type: Activity["type"]) => {
    switch (type) {
      case "order":
        return "bg-blue-500";
      case "listing":
        return "bg-green-500";
      case "message":
        return "bg-purple-500";
      default:
        return "bg-gray-500";
    }
  };

  return (
    <Card className={className}>
      <CardHeader>
        <CardTitle>Recent Activities</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {activities.length === 0 ? (
            <p className="text-sm text-muted-foreground text-center py-4">
              No recent activities
            </p>
          ) : (
            activities.map((activity, index) => (
              <div key={activity.id} className="flex gap-4">
                <div className="relative flex flex-col items-center">
                  <div className={cn(
                    "h-2 w-2 rounded-full",
                    getActivityColor(activity.type)
                  )} />
                  {index !== activities.length - 1 && (
                    <div className="h-full w-px bg-border" />
                  )}
                </div>
                <div className="flex-1 pb-4">
                  <div className="flex items-center justify-between">
                    <h4 className="text-sm font-medium">{activity.title}</h4>
                    <span className="text-xs text-muted-foreground">
                      {activity.timestamp}
                    </span>
                  </div>
                  <p className="text-sm text-muted-foreground mt-1">
                    {activity.description}
                  </p>
                </div>
              </div>
            ))
          )}
        </div>
      </CardContent>
    </Card>
  );
}

