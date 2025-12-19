import { useState } from "react";
import { Users, Mail, Calendar, MoreVertical, UserPlus, Shield } from "lucide-react";
import {
  LoadingState,
  ErrorState,
  StudioRoleBadge,
  InvitationStatusBadge,
} from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
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
import { InviteStaffDialog } from "@/components/studios";
import { ConfirmDialog } from "@/components/shared/confirm-dialog";
import {
  useStudioMembers,
  useStudioInvitations,
  useUpdateStudioMemberRole,
  useRemoveStudioMember,
  useCancelInvitation,
  useProfile,
} from "@/lib/hooks";
import { StudioRole, InvitationStatus } from "@/types";
import { canInviteStudioMembers, canManageStudioMember } from "@/lib/utils";

export default function StudioTeamPage() {
  const { user: profile } = useProfile();

  const studioId = profile?.studioId || "";

  const [inviteDialogOpen, setInviteDialogOpen] = useState(false);
  const [selectedMember, setSelectedMember] = useState<string | null>(null);
  const [confirmRemoveOpen, setConfirmRemoveOpen] = useState(false);
  const [selectedInvitation, setSelectedInvitation] = useState<string | null>(null);
  const [confirmCancelOpen, setConfirmCancelOpen] = useState(false);

  const {
    data: members,
    isLoading: membersLoading,
    error: membersError,
  } = useStudioMembers(studioId);
  const { data: invitations } = useStudioInvitations(studioId);

  const updateRole = useUpdateStudioMemberRole();
  const removeMember = useRemoveStudioMember();
  const cancelInvitation = useCancelInvitation();

  const handleRemoveMember = () => {
    if (selectedMember) {
      removeMember.mutate({ studioId, memberId: selectedMember });
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

  const canInvite = canInviteStudioMembers(profile?.studioRole);

  return (
    <div className="space-y-6">
      {/* Header with Icon */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3 mb-2">
            <div className="h-10 w-10 rounded-xl bg-purple-600 flex items-center justify-center text-white shadow-md">
              <Users className="h-5 w-5" />
            </div>
            <h1 className="text-3xl font-bold tracking-tight text-gray-900">Team Management</h1>
          </div>
          <div className="flex items-center gap-3 text-sm text-gray-600 ml-[52px]">
            <span>Manage your studio team members and invitations</span>
            {profile?.studioRole !== undefined && profile?.studioRole !== null && (
              <>
                <span>•</span>
                <div className="flex items-center gap-2">
                  <Shield className="h-3.5 w-3.5 text-gray-500" />
                  <span>Your role:</span>
                  <StudioRoleBadge role={profile.studioRole} />
                </div>
              </>
            )}
          </div>
        </div>
        {canInvite && (
          <Button onClick={() => setInviteDialogOpen(true)} className="shadow-md">
            <UserPlus className="mr-2 h-4 w-4" />
            Invite Staff
          </Button>
        )}
      </div>

      <Card className="shadow-lg">
        <CardContent className="p-6">
          <Tabs defaultValue="members">
            <TabsList className="mb-6">
              <TabsTrigger value="members">
                <Users className="h-4 w-4 mr-2" />
                Members ({members?.length || 0})
              </TabsTrigger>
              <TabsTrigger value="invitations">
                <Mail className="h-4 w-4 mr-2" />
                Invitations ({invitations?.filter((i) => i.status === InvitationStatus.Pending).length || 0})
              </TabsTrigger>
            </TabsList>

            <TabsContent value="members">
              {members && members.length > 0 ? (
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Member</TableHead>
                      <TableHead>Email</TableHead>
                      <TableHead>Role</TableHead>
                      <TableHead>Skills</TableHead>
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
                              <AvatarFallback className="bg-gradient-to-br from-purple-500 to-purple-600 text-white text-xs font-semibold">
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
                          <StudioRoleBadge role={member.role} />
                        </TableCell>
                        <TableCell>
                          {member.skills && member.skills.length > 0 ? (
                            <div className="flex flex-wrap gap-1">
                              {member.skills.slice(0, 3).map((skill, idx) => (
                                <Badge key={idx} variant="outline" className="text-xs">
                                  {skill}
                                </Badge>
                              ))}
                              {member.skills.length > 3 && (
                                <Badge variant="outline" className="text-xs">
                                  +{member.skills.length - 3}
                                </Badge>
                              )}
                            </div>
                          ) : (
                            <span className="text-gray-400">No skills</span>
                          )}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2 text-sm text-gray-600">
                            <Calendar className="h-4 w-4" />
                            {new Date(member.joinedAtUtc).toLocaleDateString()}
                          </div>
                        </TableCell>
                        <TableCell className="text-right">
                          {canManageStudioMember(profile?.studioRole, member.role) && (
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
                                      studioId,
                                      memberId: member.id,
                                      data: { newRole: StudioRole.Manager },
                                    });
                                  }}
                                >
                                  Promote to Manager
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                  onClick={() => {
                                    updateRole.mutate({
                                      studioId,
                                      memberId: member.id,
                                      data: { newRole: StudioRole.Staff },
                                    });
                                  }}
                                >
                                  Change to Staff
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
                <div className="text-center py-16 text-gray-500">
                  <div className="mx-auto h-16 w-16 rounded-full bg-gray-100 flex items-center justify-center mb-4">
                    <Users className="h-8 w-8 text-gray-400" />
                  </div>
                  <p className="text-lg font-semibold text-gray-900">No team members yet</p>
                  <p className="text-sm mt-1 text-gray-600">Invite your first team member to get started</p>
                </div>
              )}
            </TabsContent>

            <TabsContent value="invitations">
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
                <div className="text-center py-16 text-gray-500">
                  <div className="mx-auto h-16 w-16 rounded-full bg-gray-100 flex items-center justify-center mb-4">
                    <Mail className="h-8 w-8 text-gray-400" />
                  </div>
                  <p className="text-lg font-semibold text-gray-900">No invitations</p>
                  <p className="text-sm mt-1 text-gray-600">Pending invitations will appear here</p>
                </div>
              )}
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>

      <InviteStaffDialog
        studioId={studioId}
        open={inviteDialogOpen}
        onOpenChange={setInviteDialogOpen}
      />

      <ConfirmDialog
        open={confirmRemoveOpen}
        onOpenChange={setConfirmRemoveOpen}
        title="Remove Team Member"
        description="Are you sure you want to remove this member? They will lose access to the studio."
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
