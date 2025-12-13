"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { ApplicationForm } from "@/components/applications";
import { ApplicationType } from "@/types";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Building2, Camera } from "lucide-react";
import { useAuth } from "@/lib/hooks";
import { LoadingState } from "@/components/shared";

export default function ApplyPage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading } = useAuth();
  const [activeTab, setActiveTab] = useState<"agency" | "studio">("agency");

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push("/login?redirect=/profile/apply");
    }
  }, [isLoading, isAuthenticated, router]);

  const handleSuccess = () => {
    router.push("/profile?tab=applications");
  };

  if (isLoading) {
    return <LoadingState message="Loading..." />;
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  return (
    <div className="container max-w-4xl py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold">Apply for Organization</h1>
        <p className="text-gray-600 mt-2">
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
