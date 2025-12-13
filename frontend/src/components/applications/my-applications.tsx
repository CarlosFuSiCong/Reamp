"use client";

import { useMyApplications, useCancelApplication } from "@/lib/hooks";
import { ApplicationStatus, ApplicationType } from "@/types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Building2, Camera, Calendar, Mail, Phone, XCircle } from "lucide-react";
import { LoadingState, ErrorState } from "@/components/shared";
import Link from "next/link";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";

export function MyApplications() {
  const { data: applications, isLoading, error } = useMyApplications();
  const cancelMutation = useCancelApplication();

  const getStatusBadge = (status: ApplicationStatus) => {
    switch (status) {
      case ApplicationStatus.Pending:
        return <Badge variant="outline" className="bg-yellow-50 text-yellow-700 border-yellow-200">Pending</Badge>;
      case ApplicationStatus.UnderReview:
        return <Badge variant="outline" className="bg-blue-50 text-blue-700 border-blue-200">Under Review</Badge>;
      case ApplicationStatus.Approved:
        return <Badge variant="outline" className="bg-green-50 text-green-700 border-green-200">Approved</Badge>;
      case ApplicationStatus.Rejected:
        return <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200">Rejected</Badge>;
      case ApplicationStatus.Cancelled:
        return <Badge variant="outline" className="bg-gray-50 text-gray-700 border-gray-200">Cancelled</Badge>;
      default:
        return <Badge variant="outline">{ApplicationStatus[status]}</Badge>;
    }
  };

  const getTypeIcon = (type: ApplicationType) => {
    return type === ApplicationType.Agency ? (
      <Building2 className="h-5 w-5 text-blue-600" />
    ) : (
      <Camera className="h-5 w-5 text-purple-600" />
    );
  };

  const getTypeLabel = (type: ApplicationType) => {
    return type === ApplicationType.Agency ? "Agency" : "Studio";
  };

  const canCancel = (status: ApplicationStatus) => {
    return status === ApplicationStatus.Pending || status === ApplicationStatus.UnderReview;
  };

  if (isLoading) {
    return <LoadingState message="Loading applications..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load applications" />;
  }

  if (!applications || applications.length === 0) {
    return (
      <Card>
        <CardContent className="py-12 text-center">
          <div className="flex flex-col items-center gap-4">
            <div className="rounded-full bg-gray-100 p-4">
              <Building2 className="h-8 w-8 text-gray-400" />
            </div>
            <div>
              <h3 className="text-lg font-semibold">No Applications</h3>
              <p className="text-sm text-gray-600 mt-1">
                You haven't submitted any organization applications yet.
              </p>
            </div>
            <Link href="/profile/apply">
              <Button>Submit Application</Button>
            </Link>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold">My Applications</h3>
          <p className="text-sm text-gray-600">View and manage your organization applications</p>
        </div>
        <Link href="/profile/apply">
          <Button variant="outline">New Application</Button>
        </Link>
      </div>

      <div className="grid gap-4">
        {applications.map((app) => (
          <Card key={app.id}>
            <CardHeader>
              <div className="flex items-start justify-between">
                <div className="flex items-start gap-3">
                  {getTypeIcon(app.type)}
                  <div>
                    <CardTitle className="text-lg">{app.organizationName}</CardTitle>
                    <CardDescription className="mt-1">
                      {getTypeLabel(app.type)} Application
                    </CardDescription>
                  </div>
                </div>
                {getStatusBadge(app.status)}
              </div>
            </CardHeader>
            <CardContent>
              <div className="grid gap-3 text-sm">
                <div className="flex items-center gap-2 text-gray-600">
                  <Mail className="h-4 w-4" />
                  <span>{app.contactEmail}</span>
                </div>
                <div className="flex items-center gap-2 text-gray-600">
                  <Calendar className="h-4 w-4" />
                  <span>Submitted on {new Date(app.createdAtUtc).toLocaleDateString()}</span>
                </div>
                {app.reviewedAtUtc && (
                  <div className="flex items-center gap-2 text-gray-600">
                    <Calendar className="h-4 w-4" />
                    <span>Reviewed on {new Date(app.reviewedAtUtc).toLocaleDateString()}</span>
                  </div>
                )}
              </div>

              {canCancel(app.status) && (
                <div className="mt-4 pt-4 border-t">
                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button variant="outline" size="sm" className="text-red-600 hover:text-red-700">
                        <XCircle className="h-4 w-4 mr-2" />
                        Cancel Application
                      </Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                      <AlertDialogHeader>
                        <AlertDialogTitle>Cancel Application</AlertDialogTitle>
                        <AlertDialogDescription>
                          Are you sure you want to cancel this application? This action cannot be undone.
                        </AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>No, keep it</AlertDialogCancel>
                        <AlertDialogAction
                          onClick={() => cancelMutation.mutate(app.id)}
                          className="bg-red-600 hover:bg-red-700"
                        >
                          Yes, cancel
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>
              )}
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
