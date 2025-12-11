import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { LucideIcon } from "lucide-react";

interface QuickAction {
  href: string;
  label: string;
  icon: LucideIcon;
}

interface QuickActionsProps {
  actions: QuickAction[];
}

export function QuickActions({ actions }: QuickActionsProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Quick Actions</CardTitle>
      </CardHeader>
      <CardContent className="grid gap-2">
        {actions.map((action) => {
          const Icon = action.icon;
          return (
            <Link key={action.href} href={action.href}>
              <Button className="w-full justify-start" variant="outline">
                <Icon className="mr-2 h-4 w-4" />
                {action.label}
              </Button>
            </Link>
          );
        })}
      </CardContent>
    </Card>
  );
}
