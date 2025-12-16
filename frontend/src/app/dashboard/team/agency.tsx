import { useState } from "react";
import { Users, Mail, Calendar, MoreVertical, UserPlus, Shield } from "lucide-react";
import {
  PageHeader,
  LoadingState,
  ErrorState,
  AgencyRoleBadge,
  InvitationStatusBadge,
} from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { InviteMemberDialog } from "@/components/agencies";
import { ConfirmDialog } from "@/components/shared/confirm-dialog";
import {
  useAgencyMembers,
  useAgencyInvitations,
  useUpdateAgencyMemberRole,
  useRemoveAgencyMember,
  useCancelInvitation,
  useProfile,
} from "@/lib/hooks";
import { AgencyRole, InvitationStatus } from "@/types";
import { canInviteAgencyMembers, canManageAgencyMember } from "@/lib/utils";

export default function AgencyTeamPage() {
  const { user: profile } = useProfile();

  const agencyId = profile?.agencyId || "";

  const [inviteDialogOpen, setInviteDialogOpen] = useState(false);
  const [selectedMember, setSelectedMember] = useState<string | null>(null);
  const [confirmRemoveOpen, setConfirmRemoveOpen] = useState(false);
  const [selectedInvitation, setSelectedInvitation] = useState<string | null>(null);
  const [confirmCancelOpen, setConfirmCancelOpen] = useState(false);

  const {
    data: members,
    isLoading: membersLoading,
    error: membersError,
  } = useAgencyMembers(agencyId);
  const { data: invitations, isLoading: invitationsLoading } = useAgencyInvitations(agencyId);

  const updateRole = useUpdateAgencyMemberRole();
  const removeMember = useRemoveAgencyMember();
  const cancelInvitation = useCancelInvitation();

  const handleRemoveMember = () => {
    if (selectedMember) {
      removeMember.mutate({ agencyId, memberId: selectedMember });
      setConfirmRemoveOpen(false);
      setSelectedMember(null);
    }
  };

  const handleCancelInvitation = () => {
    if (selectedInvitation) {
      cancelInvitation.mutate(selectedInvitation);
      setConfirmCancelOpen(false);
      setSelectedInvitation(null);
    }
  };

  if (membersLoading) {
    return <LoadingState message="Loading team members..." />;
  }

  if (membersError) {
    return <ErrorState message="Failed to load team members" />;
  }

  const canInvite = canInviteAgencyMembers(profile?.agencyRole);

  return (
    <div>
      <PageHeader
        title="Team Management"
        description={
          <div className="flex items-center flex-wrap gap-4">
            <span>Manage your agency team members and invitations</span>
            {profile?.agencyRole !== undefined && profile?.agencyRole !== null && (
              <div className="flex items-center gap-2">
                <Shield className="h-4 w-4 text-muted-foreground" />
                <span className="text-sm">Your role:</span>
                <AgencyRoleBadge role={profile.agencyRole} />
              </div>
            )}
          </div>
        }
        action={
          canInvite ? (
            <Button onClick={() => setInviteDialogOpen(true)}>
              <UserPlus className="mr-2 h-4 w-4" />
              Invite Member
            </Button>
          ) : null
        }
      />

      <Card className="p-6">
        <Tabs defaultValue="members">
          <TabsList>
            <TabsTrigger value="members">Members ({members?.length || 0})</TabsTrigger>
            <TabsTrigger value="invitations">
              Invitations (
              {invitations?.filter((i) => i.status === InvitationStatus.Pending).length || 0})
            </TabsTrigger>
          </TabsList>

          <TabsContent value="members" className="mt-6">
            {members && members.length > 0 ? (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Member</TableHead>
                    <TableHead>Email</TableHead>
                    <TableHead>Role</TableHead>
                    <TableHead>Branch</TableHead>
                    <TableHead>Joined</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {members.map((member) => (
                    <TableRow key={member.id}>
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <Avatar>
                            <AvatarFallback>
                              {member.displayName.substring(0, 2).toUpperCase()}
                            </AvatarFallback>
                          </Avatar>
                          <span className="font-medium">{member.displayName}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2 text-gray-600">
                          <Mail className="h-4 w-4" />
                          {member.email}
                        </div>
                      </TableCell>
                      <TableCell>
                        <AgencyRoleBadge role={member.role} />
                      </TableCell>
                      <TableCell>
                        {member.agencyBranchName || (
                          <span className="text-gray-400">No branch</span>
                        )}
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2 text-sm text-gray-600">
                          <Calendar className="h-4 w-4" />
                          {new Date(member.joinedAtUtc).toLocaleDateString()}
                        </div>
                      </TableCell>
                      <TableCell className="text-right">
                        {canManageAgencyMember(profile?.agencyRole, member.role) && (
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button variant="ghost" size="sm">
                                <MoreVertical className="h-4 w-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem
                                onClick={() => {
                                  updateRole.mutate({
                                    agencyId,
                                    memberId: member.id,
                                    data: { newRole: AgencyRole.Manager },
                                  });
                                }}
                              >
                                Promote to Manager
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={() => {
                                  updateRole.mutate({
                                    agencyId,
                                    memberId: member.id,
                                    data: { newRole: AgencyRole.Agent },
                                  });
                                }}
                              >
                                Change to Agent
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={() => {
                                  setSelectedMember(member.id);
                                  setConfirmRemoveOpen(true);
                                }}
                                className="text-red-600"
                              >
                                Remove Member
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <div className="text-center py-12 text-gray-500">
                <Users className="mx-auto h-12 w-12 mb-4 text-gray-400" />
                <p className="text-lg font-medium">No team members yet</p>
                <p className="text-sm mt-1">Invite your first team member to get started</p>
              </div>
            )}
          </TabsContent>

          <TabsContent value="invitations" className="mt-6">
            {invitations && invitations.length > 0 ? (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Email</TableHead>
                    <TableHead>Role</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Invited By</TableHead>
                    <TableHead>Sent</TableHead>
                    <TableHead>Expires</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {invitations.map((invitation) => (
                    <TableRow key={invitation.id}>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Mail className="h-4 w-4 text-gray-400" />
                          {invitation.inviteeEmail}
                        </div>
                      </TableCell>
                      <TableCell>{invitation.targetRoleName}</TableCell>
                      <TableCell>
                        <InvitationStatusBadge status={invitation.status} />
                      </TableCell>
                      <TableCell>{invitation.invitedByName}</TableCell>
                      <TableCell>
                        {new Date(invitation.createdAtUtc).toLocaleDateString()}
                      </TableCell>
                      <TableCell>
                        <span className={invitation.isExpired ? "text-red-600" : ""}>
                          {new Date(invitation.expiresAtUtc).toLocaleDateString()}
                        </span>
                      </TableCell>
                      <TableCell className="text-right">
                        {invitation.status === InvitationStatus.Pending && (
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => {
                              setSelectedInvitation(invitation.id);
                              setConfirmCancelOpen(true);
                            }}
                          >
                            Cancel
                          </Button>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <div className="text-center py-12 text-gray-500">
                <Mail className="mx-auto h-12 w-12 mb-4 text-gray-400" />
                <p className="text-lg font-medium">No invitations</p>
                <p className="text-sm mt-1">Pending invitations will appear here</p>
              </div>
            )}
          </TabsContent>
        </Tabs>
      </Card>

      <InviteMemberDialog
        agencyId={agencyId}
        open={inviteDialogOpen}
        onOpenChange={setInviteDialogOpen}
      />

      <ConfirmDialog
        open={confirmRemoveOpen}
        onOpenChange={setConfirmRemoveOpen}
        title="Remove Team Member"
        description="Are you sure you want to remove this member? They will lose access to the agency."
        onConfirm={handleRemoveMember}
        confirmText="Remove"
        variant="destructive"
      />

      <ConfirmDialog
        open={confirmCancelOpen}
        onOpenChange={setConfirmCancelOpen}
        title="Cancel Invitation"
        description="Are you sure you want to cancel this invitation?"
        onConfirm={handleCancelInvitation}
        confirmText="Cancel Invitation"
      />
    </div>
  );
}
