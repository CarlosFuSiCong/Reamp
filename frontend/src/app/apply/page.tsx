"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { ApplicationForm } from "@/components/applications";
import { ApplicationType, UserRole } from "@/types";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Building2, Camera, ShieldAlert } from "lucide-react";
import { useAuth } from "@/lib/hooks";
import { LoadingState } from "@/components/shared";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import Link from "next/link";

export default function ApplyPage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading } = useAuth();
  const [activeTab, setActiveTab] = useState<"agency" | "studio">("agency");

  useEffect(() => {
    if (!isAuthenticated && !isLoading) {
      router.push("/login?redirect=/apply");
    }

    // Redirect Staff and Agent to their dashboard
    if (isAuthenticated && user && (user.role === UserRole.Staff || user.role === UserRole.Agent)) {
      router.push("/dashboard/profile");
    }
  }, [isAuthenticated, isLoading, user, router]);

  const handleSuccess = () => {
    router.push("/dashboard/profile?tab=applications");
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <LoadingState message="Loading..." />
      </div>
    );
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  // Block Staff and Agent from accessing this page
  if (user.role === UserRole.Staff || user.role === UserRole.Agent) {
    return (
      <div className="container mx-auto max-w-4xl px-4 sm:px-6 lg:px-8 py-8">
        <Alert variant="destructive">
          <ShieldAlert className="h-4 w-4" />
          <AlertTitle>Access Restricted</AlertTitle>
          <AlertDescription>
            You already belong to an organization. Only users without an organization can apply.
          </AlertDescription>
        </Alert>
        <div className="mt-6 flex gap-4">
          <Link href="/dashboard/profile">
            <Button>Go to Dashboard</Button>
          </Link>
          <Link href="/">
            <Button variant="outline">Go to Home</Button>
          </Link>
        </div>
      </div>
    );
  }

  // Admin should also not access this page
  if (user.role === UserRole.Admin) {
    return (
      <div className="container mx-auto max-w-4xl px-4 sm:px-6 lg:px-8 py-8">
        <Alert>
          <ShieldAlert className="h-4 w-4" />
          <AlertTitle>Not Applicable</AlertTitle>
          <AlertDescription>
            As an administrator, you do not need to apply for an organization.
          </AlertDescription>
        </Alert>
        <div className="mt-6 flex gap-4">
          <Link href="/admin">
            <Button>Go to Admin Panel</Button>
          </Link>
          <Link href="/">
            <Button variant="outline">Go to Home</Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto max-w-4xl px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold">Apply for Organization</h1>
        <p className="text-muted-foreground mt-2">
          Submit an application to create an Agency or Studio. Your application will be reviewed by
          our administrators.
        </p>
      </div>

      <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as "agency" | "studio")}>
        <TabsList className="grid w-full grid-cols-2">
          <TabsTrigger value="agency" className="flex items-center gap-2">
            <Building2 className="h-4 w-4" />
            Agency
          </TabsTrigger>
          <TabsTrigger value="studio" className="flex items-center gap-2">
            <Camera className="h-4 w-4" />
            Studio
          </TabsTrigger>
        </TabsList>

        <TabsContent value="agency" className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>Agency Application</CardTitle>
              <CardDescription>
                Apply to create a real estate agency. You will become the agency owner once
                approved.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <ApplicationForm
                type={ApplicationType.Agency}
                userEmail={user.email}
                onSuccess={handleSuccess}
              />
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="studio" className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>Studio Application</CardTitle>
              <CardDescription>
                Apply to create a photography studio. You will become the studio owner once
                approved.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <ApplicationForm
                type={ApplicationType.Studio}
                userEmail={user.email}
                onSuccess={handleSuccess}
              />
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
