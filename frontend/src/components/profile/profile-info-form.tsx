"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2 } from "lucide-react";
import { RoleBadge } from "@/components/shared/role-badge";
import { UserRole } from "@/types/enums";

interface ProfileFormData {
  firstName: string;
  lastName: string;
  displayName: string;
}

interface ProfileInfoFormProps {
  initialData: ProfileFormData;
  email: string;
  role: UserRole;
  onSubmit: (data: ProfileFormData) => void;
  isSubmitting?: boolean;
  isSuccess?: boolean;
}

export function ProfileInfoForm({ 
  initialData, 
  email, 
  role, 
  onSubmit, 
  isSubmitting,
  isSuccess
}: ProfileInfoFormProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState(initialData);

  useEffect(() => {
    setFormData(initialData);
  }, [initialData]);

  useEffect(() => {
    if (isSuccess) {
      setIsEditing(false);
    }
  }, [isSuccess]);

  const handleEdit = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsEditing(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    e.stopPropagation();
    
    // Only validate if values are provided
    const trimmedFirstName = formData.firstName?.trim() || "";
    const trimmedLastName = formData.lastName?.trim() || "";
    
    // If both are empty, show error
    if (!trimmedFirstName && !trimmedLastName) {
      console.error("At least one of First Name or Last Name is required");
      return;
    }
    
    console.log("Submitting profile data:", {
      firstName: trimmedFirstName,
      lastName: trimmedLastName,
    });
    
    onSubmit(formData);
  };

  const handleCancel = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setFormData(initialData);
    setIsEditing(false);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Profile Information</CardTitle>
        <CardDescription>Update your personal information</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="firstName">First Name</Label>
              <Input
                id="firstName"
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                disabled={!isEditing || isSubmitting}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="lastName">Last Name</Label>
              <Input
                id="lastName"
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                disabled={!isEditing || isSubmitting}
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="displayName">
              Display Name <span className="text-xs text-muted-foreground">(Auto-generated)</span>
            </Label>
            <Input
              id="displayName"
              value={formData.displayName}
              disabled
              className="bg-muted"
            />
          </div>

          <div className="space-y-2">
            <Label>Email</Label>
            <Input value={email} disabled />
          </div>

          <div className="space-y-2">
            <Label>Role</Label>
            <div>
              <RoleBadge role={role} />
            </div>
          </div>

          <div className="flex gap-2">
            {!isEditing ? (
              <Button type="button" onClick={handleEdit}>
                Edit Profile
              </Button>
            ) : (
              <>
                <Button type="submit" disabled={isSubmitting}>
                  {isSubmitting && <Loader2 className="h-4 w-4 animate-spin mr-2" />}
                  Save Changes
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={handleCancel}
                  disabled={isSubmitting}
                >
                  Cancel
                </Button>
              </>
            )}
          </div>
        </form>
      </CardContent>
    </Card>
  );
}

