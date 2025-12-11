"use client";

import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { User, Lock } from "lucide-react";
import { profilesApi } from "@/lib/api/profiles";
import { useAuthStore } from "@/lib/stores/auth-store";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { AvatarUpload, ProfileInfoForm, ChangePasswordForm } from "@/components/profile";
import { useUpdateProfile, useUpdateAvatar, useChangePassword } from "@/lib/hooks/use-profile";

export default function ProfilePage() {
  const { user } = useAuthStore();

  const { data: profile, isLoading, error } = useQuery({
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
      setProfileFormData({
        firstName: profile.firstName || "",
        lastName: profile.lastName || "",
        displayName: profile.displayName || "",
      });
    }
  }, [profile]);

  const updateProfileMutation = useUpdateProfile();
  const updateAvatarMutation = useUpdateAvatar(profile?.id || "");
  const changePasswordMutation = useChangePassword();

  if (isLoading) {
    return <LoadingState message="Loading profile..." />;
  }

  if (error || !profile) {
    return <ErrorState message="Failed to load profile data" />;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <PageHeader 
        title="Profile Settings" 
        description="Manage your profile and account settings" 
      />

      <Tabs defaultValue="profile" className="space-y-4">
        <TabsList>
          <TabsTrigger value="profile">
            <User className="h-4 w-4 mr-2" />
            Profile
          </TabsTrigger>
          <TabsTrigger value="security">
            <Lock className="h-4 w-4 mr-2" />
            Security
          </TabsTrigger>
        </TabsList>

        <TabsContent value="profile" className="space-y-4">
          <AvatarUpload
            avatarUrl={profile.avatarAssetId}
            displayName={profile.displayName}
            onUpload={(file) => updateAvatarMutation.mutate(file)}
            isUploading={updateAvatarMutation.isPending}
          />

          <ProfileInfoForm
            initialData={profileFormData}
            email={user?.email || ""}
            role={profile.role}
            onSubmit={(data) => updateProfileMutation.mutate(data)}
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
      </Tabs>
    </div>
  );
}

