"use client";

import { useState } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Camera, Video, Box, Save, AlertCircle } from "lucide-react";
import { StaffSkills } from "@/types/enums";
import { useUpdateStaffSkills } from "@/lib/hooks/use-staff";
import { Alert, AlertDescription } from "@/components/ui/alert";

interface StaffSkillsManagerProps {
  staffId: string;
  currentSkills: StaffSkills;
}

// Skill definitions with icons and labels
const SKILL_DEFINITIONS = [
  {
    value: StaffSkills.Photographer,
    label: "Photographer",
    icon: Camera,
    description: "Professional photography services",
  },
  {
    value: StaffSkills.Videographer,
    label: "Videographer",
    icon: Video,
    description: "Video production and filming",
  },
  {
    value: StaffSkills.VRMaker,
    label: "VR Maker",
    icon: Box,
    description: "Virtual reality content creation",
  },
];

export function StaffSkillsManager({ staffId, currentSkills }: StaffSkillsManagerProps) {
  const [selectedSkills, setSelectedSkills] = useState<StaffSkills>(currentSkills);
  const updateSkillsMutation = useUpdateStaffSkills();

  // Check if a skill is selected
  const hasSkill = (skill: StaffSkills) => {
    return (selectedSkills & skill) !== 0;
  };

  // Toggle a skill
  const toggleSkill = (skill: StaffSkills) => {
    if (hasSkill(skill)) {
      // Remove skill
      setSelectedSkills(selectedSkills & ~skill);
    } else {
      // Add skill
      setSelectedSkills(selectedSkills | skill);
    }
  };

  // Save changes
  const handleSave = () => {
    updateSkillsMutation.mutate({ staffId, skills: selectedSkills });
  };

  const hasChanges = selectedSkills !== currentSkills;
  const hasNoSkills = selectedSkills === StaffSkills.None;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Professional Skills</CardTitle>
        <CardDescription>Select the skills that you can offer as a staff member</CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Current Skills Display */}
        <div>
          <Label className="text-sm font-medium mb-3 block">Current Skills</Label>
          <div className="flex flex-wrap gap-2">
            {currentSkills === StaffSkills.None ? (
              <Badge variant="outline" className="text-muted-foreground">
                No skills selected
              </Badge>
            ) : (
              SKILL_DEFINITIONS.filter((s) => (currentSkills & s.value) !== 0).map((skill) => {
                const Icon = skill.icon;
                return (
                  <Badge key={skill.value} variant="default" className="px-3 py-1">
                    <Icon className="h-3 w-3 mr-1" />
                    {skill.label}
                  </Badge>
                );
              })
            )}
          </div>
        </div>

        {/* Skill Selection */}
        <div className="space-y-4">
          <Label className="text-sm font-medium">Select Your Skills</Label>
          <div className="space-y-3">
            {SKILL_DEFINITIONS.map((skill) => {
              const Icon = skill.icon;
              const checked = hasSkill(skill.value);

              return (
                <div
                  key={skill.value}
                  className="flex items-start space-x-3 p-4 rounded-lg border hover:bg-accent/50 transition-colors"
                >
                  <Checkbox
                    id={`skill-${skill.value}`}
                    checked={checked}
                    onCheckedChange={() => toggleSkill(skill.value)}
                  />
                  <div className="flex-1 space-y-1">
                    <Label
                      htmlFor={`skill-${skill.value}`}
                      className="flex items-center gap-2 font-medium cursor-pointer"
                    >
                      <Icon className="h-4 w-4" />
                      {skill.label}
                    </Label>
                    <p className="text-sm text-muted-foreground">{skill.description}</p>
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* Warning for no skills */}
        {hasNoSkills && hasChanges && (
          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>
              Warning: Removing all skills may prevent you from accepting certain orders.
            </AlertDescription>
          </Alert>
        )}

        {/* Save Button */}
        <div className="flex justify-end gap-2">
          {hasChanges && (
            <Button
              variant="outline"
              onClick={() => setSelectedSkills(currentSkills)}
              disabled={updateSkillsMutation.isPending}
            >
              Reset
            </Button>
          )}
          <Button onClick={handleSave} disabled={!hasChanges || updateSkillsMutation.isPending}>
            <Save className="h-4 w-4 mr-2" />
            {updateSkillsMutation.isPending ? "Saving..." : "Save Changes"}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
