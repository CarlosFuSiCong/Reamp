"use client";

import { useState } from "react";
import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useRouter } from "next/navigation";
import { format } from "date-fns";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useListings, useStudios, useCreateOrder } from "@/lib/hooks";
import { ShootTaskType } from "@/types";
import { taskTypeLabels } from "@/lib/utils/enum-labels";
import { Plus, Trash2, CalendarIcon } from "lucide-react";
import { ordersApi } from "@/lib/api";
import { cn } from "@/lib/utils";

const orderFormSchema = z.object({
  title: z.string().min(3, "Title must be at least 3 characters").max(200),
  listingId: z.string().min(1, "Please select a listing"),
  studioId: z.string().optional(), // Optional - can be left empty for marketplace claiming
  currency: z.string().min(1),
  scheduledDate: z.date().optional(),
  scheduledTime: z.string().optional(),
  tasks: z
    .array(
      z.object({
        taskType: z.nativeEnum(ShootTaskType),
        description: z.string().optional(),
        unitPrice: z.number().min(0, "Price must be positive"),
      })
    )
    .min(1, "At least one task is required"),
});

type OrderFormValues = z.infer<typeof orderFormSchema>;

export function OrderCreationForm() {
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { data: listingsData, isLoading: isLoadingListings } = useListings({ pageSize: 100 });
  const { data: studiosData, isLoading: isLoadingStudios } = useStudios({ pageSize: 100 });
  const createMutation = useCreateOrder();

  const isLoadingData = isLoadingListings || isLoadingStudios;

  const form = useForm<OrderFormValues>({
    resolver: zodResolver(orderFormSchema),
    defaultValues: {
      title: "",
      currency: "AUD",
      tasks: [
        {
          taskType: ShootTaskType.Photography,
          description: "",
          unitPrice: 0,
        },
      ],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "tasks",
  });

  const watchTasks = form.watch("tasks");
  const totalAmount = watchTasks?.reduce((sum, task) => sum + (task.unitPrice || 0), 0) || 0;

  const onSubmit = async (values: OrderFormValues) => {
    setIsSubmitting(true);
    try {
      // Create the order (agencyId and studioId are both optional on frontend)
      // Backend will auto-populate agencyId from the current user's agent record
      const orderData: any = {
        title: values.title,
        listingId: values.listingId,
        currency: values.currency,
      };

      // Only include studioId if a studio was selected (not "none")
      if (values.studioId && values.studioId !== "none") {
        orderData.studioId = values.studioId;
      }

      // Include scheduled date if provided
      if (values.scheduledDate) {
        orderData.scheduledStartUtc = values.scheduledDate.toISOString();
      }

      console.log("📤 Submitting order data:", orderData);
      const result = await createMutation.mutateAsync(orderData);

      // Add tasks to the order
      for (const task of values.tasks) {
        await ordersApi.addTask(result.id, {
          type: task.taskType,
          notes: task.description,
          price: task.unitPrice,
        });
      }

      router.push(`/dashboard/orders/${result.id}`);
    } catch (error) {
      console.error("Failed to create order:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoadingData) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto mb-4"></div>
          <p className="text-muted-foreground">Loading form data...</p>
        </div>
      </div>
    );
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle>Basic Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Order Title</FormLabel>
                  <FormControl>
                    <Input placeholder="e.g., Property Photography - 123 Main St" {...field} />
                  </FormControl>
                  <FormDescription>Give this order a descriptive title</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="listingId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Listing</FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a listing" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {listingsData?.items.length === 0 && (
                        <SelectItem value="no-listings" disabled>
                          No listings available
                        </SelectItem>
                      )}
                      {listingsData?.items.map((listing) => (
                        <SelectItem key={listing.id} value={listing.id}>
                          {listing.title}
                          {listing.city ? ` - ${listing.city}` : ""}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormDescription>
                    Select the property listing for this photography order
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="studioId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Photography Studio (Optional)</FormLabel>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Leave empty to publish to marketplace" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value="none">No studio (publish to marketplace)</SelectItem>
                      {!studiosData?.items || studiosData.items.length === 0 ? (
                        <SelectItem value="no-studios" disabled>
                          No studios available
                        </SelectItem>
                      ) : (
                        studiosData.items.map((studio) => (
                          <SelectItem key={studio.id} value={studio.id}>
                            {studio.name}
                          </SelectItem>
                        ))
                      )}
                    </SelectContent>
                  </Select>
                  <FormDescription>
                    Choose a specific studio or leave empty to let studios claim this order
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="scheduledDate"
              render={({ field }) => (
                <FormItem className="flex flex-col">
                  <FormLabel>Scheduled Date & Time (Optional)</FormLabel>
                  <Popover>
                    <PopoverTrigger asChild>
                      <FormControl>
                        <Button
                          variant={"outline"}
                          className={cn(
                            "w-full pl-3 text-left font-normal",
                            !field.value && "text-muted-foreground"
                          )}
                        >
                          {field.value ? format(field.value, "PPP") : <span>Pick a date</span>}
                          <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                        </Button>
                      </FormControl>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                      <Calendar
                        mode="single"
                        selected={field.value}
                        onSelect={field.onChange}
                        disabled={(date) => date < new Date(new Date().setHours(0, 0, 0, 0))}
                        initialFocus
                      />
                    </PopoverContent>
                  </Popover>
                  <div className="flex gap-2">
                    <Input
                      type="time"
                      className="flex-1"
                      onChange={(e) => {
                        if (field.value && e.target.value) {
                          const [hours, minutes] = e.target.value.split(":");
                          const newDate = new Date(field.value);
                          newDate.setHours(parseInt(hours), parseInt(minutes));
                          field.onChange(newDate);
                        }
                      }}
                      value={field.value ? format(field.value, "HH:mm") : ""}
                    />
                  </div>
                  <FormDescription>When should the photoshoot take place?</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="currency"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Currency</FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value="AUD">AUD</SelectItem>
                      <SelectItem value="USD">USD</SelectItem>
                      <SelectItem value="EUR">EUR</SelectItem>
                      <SelectItem value="GBP">GBP</SelectItem>
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardTitle>Photography Tasks</CardTitle>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() =>
                  append({
                    taskType: ShootTaskType.Photography,
                    description: "",
                    unitPrice: 0,
                  })
                }
              >
                <Plus className="mr-2 h-4 w-4" />
                Add Task
              </Button>
            </div>
          </CardHeader>
          <CardContent className="space-y-4">
            {fields.map((field, index) => (
              <Card key={field.id}>
                <CardContent className="pt-6">
                  <div className="space-y-4">
                    <div className="flex items-start justify-between gap-4">
                      <div className="flex-1 space-y-4">
                        <FormField
                          control={form.control}
                          name={`tasks.${index}.taskType`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Task Type</FormLabel>
                              <Select
                                onValueChange={(value) => field.onChange(parseInt(value))}
                                defaultValue={field.value?.toString()}
                              >
                                <FormControl>
                                  <SelectTrigger>
                                    <SelectValue />
                                  </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                  {Object.entries(taskTypeLabels)
                                    .filter(([key]) => parseInt(key) !== ShootTaskType.None)
                                    .map(([key, label]) => (
                                      <SelectItem key={key} value={key}>
                                        {label}
                                      </SelectItem>
                                    ))}
                                </SelectContent>
                              </Select>
                              <FormMessage />
                            </FormItem>
                          )}
                        />

                        <FormField
                          control={form.control}
                          name={`tasks.${index}.description`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Description (Optional)</FormLabel>
                              <FormControl>
                                <Textarea
                                  placeholder="Add any specific requirements..."
                                  {...field}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />

                        <FormField
                          control={form.control}
                          name={`tasks.${index}.unitPrice`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>Price</FormLabel>
                              <FormControl>
                                <Input
                                  type="number"
                                  step="0.01"
                                  min="0"
                                  placeholder="0.00"
                                  {...field}
                                  value={field.value || 0}
                                  onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      </div>

                      {fields.length > 1 && (
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon"
                          onClick={() => remove(index)}
                        >
                          <Trash2 className="h-4 w-4 text-destructive" />
                        </Button>
                      )}
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}

            <div className="flex items-center justify-between pt-4 border-t">
              <span className="text-lg font-semibold">Total Amount:</span>
              <span className="text-2xl font-bold">
                {form.watch("currency")} {totalAmount.toFixed(2)}
              </span>
            </div>
          </CardContent>
        </Card>

        <div className="flex justify-end gap-4">
          <Button
            type="button"
            variant="outline"
            onClick={() => router.back()}
            disabled={isSubmitting}
          >
            Cancel
          </Button>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Publishing..." : "Publish Order"}
          </Button>
        </div>
      </form>
    </Form>
  );
}
