import { PageHeader } from "@/components/shared/page-header";

export default function AgentListingsPage() {
  return (
    <div>
      <PageHeader
        title="My Listings"
        description="Manage your property listings"
      />
      <div className="flex items-center justify-center h-64">
        <p className="text-muted-foreground">Coming soon...</p>
      </div>
    </div>
  );
}

