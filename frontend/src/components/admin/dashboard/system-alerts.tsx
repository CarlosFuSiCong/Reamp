import { AlertCircle } from "lucide-react";

interface SystemAlertsProps {
  alerts: Array<{
    title: string;
    message: string;
  }>;
}

export function SystemAlerts({ alerts }: SystemAlertsProps) {
  if (!alerts || alerts.length === 0) {
    return (
      <p className="text-sm text-muted-foreground text-center py-4">
        No system alerts
      </p>
    );
  }

  return (
    <div className="space-y-2">
      {alerts.map((alert, index) => (
        <div
          key={index}
          className="flex items-start gap-3 rounded-lg border border-border bg-card p-3"
        >
          <AlertCircle className="h-5 w-5 text-destructive mt-0.5" />
          <div className="flex-1">
            <p className="text-sm font-medium text-card-foreground">{alert.title}</p>
            <p className="text-xs text-muted-foreground">
              {alert.message}
            </p>
          </div>
        </div>
      ))}
    </div>
  );
}
