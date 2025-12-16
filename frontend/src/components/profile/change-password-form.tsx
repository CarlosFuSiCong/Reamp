"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2 } from "lucide-react";

interface PasswordFormData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

interface ChangePasswordFormProps {
  onSubmit: (data: PasswordFormData) => void;
  isSubmitting?: boolean;
  isSuccess?: boolean;
}

export function ChangePasswordForm({ onSubmit, isSubmitting, isSuccess }: ChangePasswordFormProps) {
  const [formData, setFormData] = useState<PasswordFormData>({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  useEffect(() => {
    if (isSuccess) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setFormData({ currentPassword: "", newPassword: "", confirmPassword: "" });
    }
  }, [isSuccess]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.newPassword !== formData.confirmPassword) {
      alert("New password and confirm password do not match");
      return;
    }

    if (formData.newPassword.length < 6) {
      alert("Password must be at least 6 characters long");
      return;
    }

    if (!/[^a-zA-Z0-9]/.test(formData.newPassword)) {
      alert("Password must contain at least one special character");
      return;
    }

    onSubmit(formData);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Change Password</CardTitle>
        <CardDescription>Update your account password</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="currentPassword">Current Password</Label>
            <Input
              id="currentPassword"
              type="password"
              value={formData.currentPassword}
              onChange={(e) => setFormData({ ...formData, currentPassword: e.target.value })}
              disabled={isSubmitting}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="newPassword">New Password</Label>
            <Input
              id="newPassword"
              type="password"
              value={formData.newPassword}
              onChange={(e) => setFormData({ ...formData, newPassword: e.target.value })}
              disabled={isSubmitting}
              required
            />
            <p className="text-xs text-gray-500">
              Must be at least 6 characters and contain a special character (!@#$%^&*)
            </p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="confirmPassword">Confirm New Password</Label>
            <Input
              id="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
              disabled={isSubmitting}
              required
            />
          </div>

          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting && <Loader2 className="h-4 w-4 animate-spin mr-2" />}
            Change Password
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
