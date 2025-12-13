"use client";

import { useState } from "react";
import { Mail, Calendar, Building2, Camera, Check, X } from "lucide-react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { LoadingState, ErrorState } from "@/components/shared";
import { ConfirmDialog } from "@/components/shared/confirm-dialog";
import {
  useMyInvitations,
  useAcceptInvitation,
  useRejectInvitation,
} from "@/lib/hooks";
import { InvitationType, InvitationStatus } from "@/types";

export function MyInvitations() {
  const { data: invitations, isLoading, error } = useMyInvitations();
  const [selectedInvitation, setSelectedInvitation] = useState<string | null>(null);
  const [confirmAcceptOpen, setConfirmAcceptOpen] = useState(false);
  const [confirmRejectOpen, setConfirmRejectOpen] = useState(false);

  const acceptInvitation = useAcceptInvitation();
  const rejectInvitation = useRejectInvitation();

  const getTypeIcon = (type: InvitationType) => {
    return type === InvitationType.Agency ? (
      <Building2 className="h-5 w-5 text-blue-600" />
    ) : (
      <Camera className="h-5 w-5 text-purple-600" />
    );
  };

  const getTypeLabel = (type: InvitationType) => {
    return type === InvitationType.Agency ? "Agency" : "Studio";
  };

  const getStatusBadge = (status: InvitationStatus, isExpired: boolean) => {
    if (isExpired) {
      return <Badge className="bg-gray-50 text-gray-700 border-gray-200">Expired</Badge>;
    }

    switch (status) {
      case InvitationStatus.Pending:
        return <Badge className="bg-yellow-50 text-yellow-700 border-yellow-200">Pending</Badge>;
      case InvitationStatus.Accepted:
        return <Badge className="bg-green-50 text-green-700 border-green-200">Accepted</Badge>;
      case InvitationStatus.Rejected:
        return <Badge className="bg-red-50 text-red-700 border-red-200">Rejected</Badge>;
      case InvitationStatus.Cancelled:
        return <Badge className="bg-gray-50 text-gray-700 border-gray-200">Cancelled</Badge>;
      default:
        return <Badge>{status}</Badge>;
    }
  };

  const handleAccept = () => {
    if (selectedInvitation) {
      acceptInvitation.mutate(selectedInvitation);
      setConfirmAcceptOpen(false);
      setSelectedInvitation(null);
    }
  };

  const handleReject = () => {
    if (selectedInvitation) {
      rejectInvitation.mutate(selectedInvitation);
      setConfirmRejectOpen(false);
      setSelectedInvitation(null);
    }
  };

  if (isLoading) {
    return <LoadingState message="Loading invitations..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load invitations" />;
  }

  const pendingInvitations = invitations?.filter(
    (inv) => inv.status === InvitationStatus.Pending && !inv.isExpired
  );
  const otherInvitations = invitations?.filter(
    (inv) => inv.status !== InvitationStatus.Pending || inv.isExpired
  );

  return (
    <div className="space-y-6">
      {/* Pending Invitations */}
      {pendingInvitations && pendingInvitations.length > 0 && (
        <Card className="p-6">
          <h3 className="text-lg font-semibold mb-4">Pending Invitations</h3>
          <div className="space-y-4">
            {pendingInvitations.map((invitation) => (
              <div
                key={invitation.id}
                className="border rounded-lg p-4 hover:border-gray-300 transition-colors"
              >
                <div className="flex items-start justify-between">
                  <div className="flex items-start gap-3 flex-1">
                    {getTypeIcon(invitation.type)}
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-1">
                        <h4 className="font-medium text-gray-900">
                          {invitation.targetEntityName}
                        </h4>
                        <Badge variant="outline" className="text-xs">
                          {getTypeLabel(invitation.type)}
                        </Badge>
                      </div>
                      <p className="text-sm text-gray-600 mb-2">
                        You've been invited as <span className="font-medium">{invitation.targetRoleName}</span>
                      </p>
                      <div className="flex items-center gap-4 text-xs text-gray-500">
                        <div className="flex items-center gap-1">
                          <Calendar className="h-3 w-3" />
                          Expires: {new Date(invitation.expiresAtUtc).toLocaleDateString()}
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="flex gap-2 ml-4">
                    <Button
                      size="sm"
                      onClick={() => {
                        setSelectedInvitation(invitation.id);
                        setConfirmAcceptOpen(true);
                      }}
                      disabled={acceptInvitation.isPending || rejectInvitation.isPending}
                    >
                      <Check className="h-4 w-4 mr-1" />
                      Accept
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => {
                        setSelectedInvitation(invitation.id);
                        setConfirmRejectOpen(true);
                      }}
                      disabled={acceptInvitation.isPending || rejectInvitation.isPending}
                    >
                      <X className="h-4 w-4 mr-1" />
                      Decline
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </Card>
      )}

      {/* Invitation History */}
      {otherInvitations && otherInvitations.length > 0 && (
        <Card className="p-6">
          <h3 className="text-lg font-semibold mb-4">Invitation History</h3>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Organization</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Role</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Received</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {otherInvitations.map((invitation) => (
                <TableRow key={invitation.id}>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      {getTypeIcon(invitation.type)}
                      <span className="font-medium">{invitation.targetEntityName}</span>
                    </div>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{getTypeLabel(invitation.type)}</Badge>
                  </TableCell>
                  <TableCell>{invitation.targetRoleName}</TableCell>
                  <TableCell>
                    {getStatusBadge(invitation.status, invitation.isExpired)}
                  </TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2 text-sm text-gray-600">
                      <Calendar className="h-4 w-4" />
                      {new Date(invitation.createdAtUtc).toLocaleDateString()}
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Card>
      )}

      {/* Empty State */}
      {(!invitations || invitations.length === 0) && (
        <Card className="p-12">
          <div className="text-center">
            <Mail className="mx-auto h-12 w-12 mb-4 text-gray-400" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No Invitations</h3>
            <p className="text-sm text-gray-500">
              You don't have any invitations at the moment
            </p>
          </div>
        </Card>
      )}

      {/* Confirm Dialogs */}
      <ConfirmDialog
        open={confirmAcceptOpen}
        onOpenChange={setConfirmAcceptOpen}
        title="Accept Invitation"
        description="Are you sure you want to accept this invitation? You will become a member of this organization."
        onConfirm={handleAccept}
        confirmText="Accept"
      />

      <ConfirmDialog
        open={confirmRejectOpen}
        onOpenChange={setConfirmRejectOpen}
        title="Decline Invitation"
        description="Are you sure you want to decline this invitation?"
        onConfirm={handleReject}
        confirmText="Decline"
        variant="destructive"
      />
    </div>
  );
}
