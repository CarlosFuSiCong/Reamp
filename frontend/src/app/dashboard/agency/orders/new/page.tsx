"use client";

import { PageHeader } from "@/components/shared";
import { OrderCreationForm } from "@/components/orders/order-creation-form";

export default function NewOrderPage() {
  return (
    <div className="space-y-6">
      <PageHeader
        title="Create New Order"
        description="Create a new photography order for your listing"
      />
      <OrderCreationForm />
    </div>
  );
}
