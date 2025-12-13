import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { UsersTable, AgenciesTable, StudiosTable } from "@/components/admin";
import { ApplicationsList } from "@/components/applications";
import { SystemAlerts } from "./system-alerts";
import { AlertCircle } from "lucide-react";

interface ManagementTabsProps {
  alerts: Array<{
    title: string;
    message: string;
  }>;
}

export function ManagementTabs({ alerts }: ManagementTabsProps) {
  return (
    <Tabs defaultValue="applications" className="space-y-4">
      <TabsList>
        <TabsTrigger value="applications">Applications</TabsTrigger>
        <TabsTrigger value="users">Users</TabsTrigger>
        <TabsTrigger value="agencies">Agencies</TabsTrigger>
        <TabsTrigger value="studios">Studios</TabsTrigger>
        <TabsTrigger value="alerts">System Alerts</TabsTrigger>
      </TabsList>

      <TabsContent value="applications" className="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Organization Applications</CardTitle>
          </CardHeader>
          <CardContent>
            <ApplicationsList />
          </CardContent>
        </Card>
      </TabsContent>

      <TabsContent value="users" className="space-y-4">
        <UsersTable />
      </TabsContent>

      <TabsContent value="agencies" className="space-y-4">
        <AgenciesTable />
      </TabsContent>

      <TabsContent value="studios" className="space-y-4">
        <StudiosTable />
      </TabsContent>

      <TabsContent value="alerts" className="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <AlertCircle className="h-5 w-5" />
              System Alerts
            </CardTitle>
          </CardHeader>
          <CardContent>
            <SystemAlerts alerts={alerts} />
          </CardContent>
        </Card>
      </TabsContent>
    </Tabs>
  );
}
