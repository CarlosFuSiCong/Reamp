import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { StatsChart } from "@/components/admin/stats-chart";
import { ActivityTimeline, Activity } from "@/components/dashboard/activity-timeline";

interface ActivitySectionProps {
  chartData: Array<{
    date: string;
    orders: number;
    listings: number;
  }>;
  activities: Activity[];
}

export function ActivitySection({ chartData, activities }: ActivitySectionProps) {
  return (
    <div className="grid gap-4 lg:grid-cols-7">
      <Card className="lg:col-span-4">
        <CardHeader>
          <CardTitle>System Activity</CardTitle>
        </CardHeader>
        <CardContent>
          <StatsChart data={chartData} />
        </CardContent>
      </Card>

      <Card className="lg:col-span-3">
        <CardHeader>
          <CardTitle>Recent Activity</CardTitle>
        </CardHeader>
        <CardContent>
          <ActivityTimeline activities={activities} />
        </CardContent>
      </Card>
    </div>
  );
}
