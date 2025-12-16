"use client";

import { use, useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { User, Lock, FileText, Mail, Briefcase } from "lucide-react";
import { profilesApi } from "@/lib/api/profiles";
import { useAuthStore } from "@/lib/stores/auth-store";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import {
  AvatarUpload,
  ProfileInfoForm,
  ChangePasswordForm,
  MyInvitations,
  StaffSkillsManager,
} from "@/components/profile";
import { MyApplications } from "@/components/applications";
import { useUpdateProfile, useUpdateAvatar, useChangePassword } from "@/lib/hooks/use-profile";
import { useStaffByUserProfileId } from "@/lib/hooks/use-staff";
import { UserRole } from "@/types/enums";

export default function ProfilePage({ searchParams }: { searchParams: Promise<{ tab?: string }> }) {
  const { user } = useAuthStore();
  const resolvedSearchParams = use(searchParams);
  const defaultTab = resolvedSearchParams?.tab || "profile";

  const {
    data: profile,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["profile"],
    queryFn: profilesApi.getMe,
  });

  const [profileFormData, setProfileFormData] = useState({
    firstName: "",
    lastName: "",
    displayName: "",
  });

  useEffect(() => {
    if (profile) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setProfileFormData({
        firstName: profile.firstName || "",
        lastName: profile.lastName || "",
        displayName: profile.displayName || "",
      });
    }
  }, [profile]);

  const updateProfileMutation = useUpdateProfile();
  const updateAvatarMutation = useUpdateAvatar();
  const changePasswordMutation = useChangePassword();

  // Fetch staff data if user is a staff member
  const isStaffUser = profile?.role === UserRole.Staff;
  const { data: staffData } = useStaffByUserProfileId(isStaffUser ? profile?.id || null : null);

  if (isLoading) {
    return <LoadingState message="Loading profile..." />;
  }

  if (error || !profile) {
    return <ErrorState message="Failed to load profile data" />;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <PageHeader title="Profile Settings" description="Manage your profile and account settings" />

      <Tabs defaultValue={defaultTab} className="space-y-4">
        <TabsList>
          <TabsTrigger value="profile">
            <User className="h-4 w-4 mr-2" />
            Profile
          </TabsTrigger>
          <TabsTrigger value="security">
            <Lock className="h-4 w-4 mr-2" />
            Security
          </TabsTrigger>
          {isStaffUser && staffData && (
            <TabsTrigger value="skills">
              <Briefcase className="h-4 w-4 mr-2" />
              Skills
            </TabsTrigger>
          )}
          <TabsTrigger value="invitations">
            <Mail className="h-4 w-4 mr-2" />
            Invitations
          </TabsTrigger>
          <TabsTrigger value="applications">
            <FileText className="h-4 w-4 mr-2" />
            Applications
          </TabsTrigger>
        </TabsList>

        <TabsContent value="profile" className="space-y-4">
          <AvatarUpload
            avatarAssetId={profile.avatarAssetId}
            displayName={profile.displayName}
            onUpload={(assetId) => updateAvatarMutation.mutate({ profileId: profile.id, assetId })}
            isUploading={updateAvatarMutation.isPending}
          />

          <ProfileInfoForm
            initialData={profileFormData}
            email={user?.email || ""}
            role={profile.role}
            onSubmit={(data) =>
              updateProfileMutation.mutate({
                profileId: profile.id,
                data: {
                  firstName: data.firstName,
                  lastName: data.lastName,
                },
              })
            }
            isSubmitting={updateProfileMutation.isPending}
            isSuccess={updateProfileMutation.isSuccess}
          />
        </TabsContent>

        <TabsContent value="security">
          <ChangePasswordForm
            onSubmit={(data) => changePasswordMutation.mutate(data)}
            isSubmitting={changePasswordMutation.isPending}
            isSuccess={changePasswordMutation.isSuccess}
          />
        </TabsContent>

        {isStaffUser && staffData && (
          <TabsContent value="skills">
            <StaffSkillsManager staffId={staffData.id} currentSkills={staffData.skills} />
          </TabsContent>
        )}

        <TabsContent value="invitations">
          <MyInvitations />
        </TabsContent>

        <TabsContent value="applications">
          <MyApplications />
        </TabsContent>
      </Tabs>
    </div>
  );
}
