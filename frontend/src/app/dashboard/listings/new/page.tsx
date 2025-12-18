"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { PageHeader } from "@/components/shared";
import { ChunkedUpload } from "@/components/media";
import { useCreateListing } from "@/lib/hooks/use-listings";
import { useProfile } from "@/lib/hooks";
import { ListingType, PropertyType, MediaAssetDetailDto } from "@/types";
import { ChevronLeft, ChevronRight, Save, Image as ImageIcon, Trash2 } from "lucide-react";

const listingSchema = z.object({
  title: z.string().min(5, "Title must be at least 5 characters"),
  description: z
    .string()
    .max(5000, "Description cannot exceed 5000 characters")
    .optional()
    .or(z.literal("")),
  price: z.number().min(0, "Price must be positive"),
  currency: z.string().min(3, "Currency is required"),
  listingType: z.nativeEnum(ListingType),
  propertyType: z.nativeEnum(PropertyType),
  bedrooms: z.number().min(0, "Must be 0 or more"),
  bathrooms: z.number().min(0, "Must be 0 or more"),
  parkingSpaces: z.number().min(0, "Must be 0 or more"),
  floorAreaSqm: z.number().optional(),
  landAreaSqm: z.number().optional(),
  addressLine1: z.string().min(5, "Address is required"),
  addressLine2: z.string().optional(),
  city: z.string().min(2, "City is required"),
  state: z.string().min(2, "State is required"),
  postcode: z.string().min(3, "Postcode is required"),
  country: z.string().min(2, "Country is required"),
});

type ListingFormValues = z.infer<typeof listingSchema>;

const STEPS = [
  { id: 1, name: "Basic Info" },
  { id: 2, name: "Address" },
  { id: 3, name: "Details" },
  { id: 4, name: "Media" },
];

export default function NewListingPage() {
  const router = useRouter();
  const [currentStep, setCurrentStep] = useState(1);
  const [uploadedMedia, setUploadedMedia] = useState<MediaAssetDetailDto[]>([]);
  const createMutation = useCreateListing();
  const { user } = useProfile();

  const form = useForm<ListingFormValues>({
    resolver: zodResolver(listingSchema),
    defaultValues: {
      title: "",
      description: "",
      price: 0,
      currency: "AUD",
      listingType: ListingType.ForSale,
      propertyType: PropertyType.House,
      bedrooms: 0,
      bathrooms: 0,
      parkingSpaces: 0,
      floorAreaSqm: undefined,
      landAreaSqm: undefined,
      addressLine1: "",
      addressLine2: "",
      city: "",
      state: "",
      postcode: "",
      country: "Australia",
    },
  });

  const onSubmit = async (data: ListingFormValues) => {
    console.log("📝 onSubmit called - Current step:", currentStep, "Total steps:", STEPS.length);
    console.log("📝 Stack trace:", new Error().stack);
    // Only allow submission on the last step
    if (currentStep !== STEPS.length) {
      console.log("❌ Form submission blocked - not on final step");
      return;
    }

    // Validate all fields before submission
    const isValid = await form.trigger();
    if (!isValid) {
      console.log("❌ Form validation failed - please check all fields");
      return;
    }

    console.log("✅ Proceeding with form submission");

    // Transform form data to API format
    const apiData = {
      title: data.title,
      description: data.description,
      price: data.price,
      currency: data.currency,
      listingType: data.listingType,
      propertyType: data.propertyType,
      address: {
        line1: data.addressLine1,
        line2: data.addressLine2 || undefined,
        city: data.city,
        state: data.state,
        postcode: data.postcode,
        country: "AU", // Convert "Australia" to ISO code
      },
      bedrooms: data.bedrooms,
      bathrooms: data.bathrooms,
      parkingSpaces: data.parkingSpaces,
      floorAreaSqm: data.floorAreaSqm,
      landAreaSqm: data.landAreaSqm,
    };

    createMutation.mutate(apiData, {
      onSuccess: () => {
        router.push("/dashboard/listings");
      },
    });
  };

  const nextStep = async (e?: React.MouseEvent) => {
    e?.preventDefault();
    e?.stopPropagation();
    console.log("➡️ nextStep called - Current step:", currentStep);
    let fieldsToValidate: (keyof ListingFormValues)[] = [];

    if (currentStep === 1) {
      fieldsToValidate = ["title", "listingType", "propertyType", "price", "currency"];
    } else if (currentStep === 2) {
      fieldsToValidate = ["addressLine1", "city", "state", "postcode", "country"];
    }

    const isValid = await form.trigger(fieldsToValidate);
    console.log("Validation result:", isValid, "for fields:", fieldsToValidate);
    if (isValid && currentStep < STEPS.length) {
      console.log("✅ Moving to step:", currentStep + 1);
      setCurrentStep(currentStep + 1);
    }
  };

  const prevStep = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLFormElement>) => {
    console.log("🔑 Key pressed:", e.key, "Current step:", currentStep, "Target:", e.target);
    // Prevent form submission on Enter key unless on the last step
    if (e.key === "Enter") {
      e.preventDefault();
      console.log("✋ Prevented Enter key default behavior");

      if (currentStep !== STEPS.length) {
        console.log("➡️ Triggering next step from Enter key");
        nextStep();
      } else {
        console.log("📝 On final step, will submit via button click");
      }
    }
  };

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      <PageHeader title="Create New Listing" description="Add a new property listing" />

      <div className="flex justify-between mb-8">
        {STEPS.map((step) => (
          <div
            key={step.id}
            className={`flex-1 ${step.id < STEPS.length ? "border-r" : ""} border-gray-200 pb-4`}
          >
            <div className="flex flex-col items-center">
              <div
                className={`w-10 h-10 rounded-full flex items-center justify-center mb-2 ${
                  currentStep >= step.id
                    ? "bg-primary text-primary-foreground"
                    : "bg-muted text-muted-foreground"
                }`}
              >
                {step.id}
              </div>
              <div
                className={`text-sm font-medium ${
                  currentStep >= step.id ? "text-foreground" : "text-muted-foreground"
                }`}
              >
                {step.name}
              </div>
            </div>
          </div>
        ))}
      </div>

      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          onKeyDown={handleKeyDown}
          className="space-y-6"
        >
          {currentStep === 1 && (
            <Card>
              <CardHeader>
                <CardTitle>Basic Information</CardTitle>
                <CardDescription>Enter the basic details of the property</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <FormField
                  control={form.control}
                  name="title"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Title</FormLabel>
                      <FormControl>
                        <Input placeholder="Beautiful 3BR house with garden" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="listingType"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Transaction Type</FormLabel>
                        <Select
                          value={field.value?.toString()}
                          onValueChange={(value) => field.onChange(parseInt(value))}
                        >
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value={ListingType.ForSale.toString()}>For Sale</SelectItem>
                            <SelectItem value={ListingType.ForRent.toString()}>For Rent</SelectItem>
                            <SelectItem value={ListingType.Auction.toString()}>Auction</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="propertyType"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Property Type</FormLabel>
                        <Select
                          value={field.value?.toString()}
                          onValueChange={(value) => field.onChange(parseInt(value))}
                        >
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value={PropertyType.House.toString()}>House</SelectItem>
                            <SelectItem value={PropertyType.Apartment.toString()}>
                              Apartment
                            </SelectItem>
                            <SelectItem value={PropertyType.Townhouse.toString()}>
                              Townhouse
                            </SelectItem>
                            <SelectItem value={PropertyType.Villa.toString()}>Villa</SelectItem>
                            <SelectItem value={PropertyType.Others.toString()}>Others</SelectItem>
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="price"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Price</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            placeholder="500000"
                            {...field}
                            onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                          />
                        </FormControl>
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
                        <Select value={field.value} onValueChange={field.onChange}>
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
                </div>

                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Description (Optional)</FormLabel>
                      <FormControl>
                        <Textarea
                          placeholder="Detailed description of the property..."
                          rows={5}
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </CardContent>
            </Card>
          )}

          {currentStep === 2 && (
            <Card>
              <CardHeader>
                <CardTitle>Address Information</CardTitle>
                <CardDescription>Enter the property location</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <FormField
                  control={form.control}
                  name="addressLine1"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Street Address</FormLabel>
                      <FormControl>
                        <Input placeholder="123 Main Street" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="addressLine2"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Address Line 2 (Optional)</FormLabel>
                      <FormControl>
                        <Input placeholder="Unit 5" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="city"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>City</FormLabel>
                        <FormControl>
                          <Input placeholder="Sydney" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="state"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>State</FormLabel>
                        <FormControl>
                          <Input placeholder="NSW" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="postcode"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Postcode</FormLabel>
                        <FormControl>
                          <Input placeholder="2000" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="country"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Country</FormLabel>
                        <FormControl>
                          <Input placeholder="Australia" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </CardContent>
            </Card>
          )}

          {currentStep === 3 && (
            <Card>
              <CardHeader>
                <CardTitle>Property Details</CardTitle>
                <CardDescription>Additional property specifications</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-3 gap-4">
                  <FormField
                    control={form.control}
                    name="bedrooms"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Bedrooms</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            min="0"
                            {...field}
                            onChange={(e) => {
                              const value = e.target.value;
                              field.onChange(value === "" ? 0 : parseInt(value));
                            }}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="bathrooms"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Bathrooms</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            min="0"
                            {...field}
                            onChange={(e) => {
                              const value = e.target.value;
                              field.onChange(value === "" ? 0 : parseInt(value));
                            }}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="parkingSpaces"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Parking</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            min="0"
                            {...field}
                            onChange={(e) => field.onChange(parseInt(e.target.value) || 0)}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <Separator />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="floorAreaSqm"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Floor Area (sqm) - Optional</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            min="0"
                            placeholder="150"
                            {...field}
                            value={field.value || ""}
                            onChange={(e) =>
                              field.onChange(
                                e.target.value ? parseFloat(e.target.value) : undefined
                              )
                            }
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="landAreaSqm"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Land Area (sqm) - Optional</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            min="0"
                            placeholder="500"
                            {...field}
                            value={field.value || ""}
                            onChange={(e) =>
                              field.onChange(
                                e.target.value ? parseFloat(e.target.value) : undefined
                              )
                            }
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </CardContent>
            </Card>
          )}

          {currentStep === 4 && (
            <Card>
              <CardHeader>
                <CardTitle>Media Upload</CardTitle>
                <CardDescription>
                  Upload photos and videos of the property (optional - you can add media later)
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                {user?.studioId ? (
                  <ChunkedUpload
                    ownerStudioId={user.studioId}
                    onUploadComplete={(asset) => {
                      setUploadedMedia((prev) => [...prev, asset]);
                    }}
                    accept="image/*,video/*"
                    maxFiles={20}
                    maxSizeMB={100}
                  />
                ) : (
                  <div className="text-center py-8 text-muted-foreground">
                    <ImageIcon className="mx-auto h-12 w-12 mb-4 opacity-50" />
                    <p>Media upload is only available for Studio members.</p>
                    <p className="text-sm mt-2">
                      You can add media after creating the listing by ordering a photoshoot.
                    </p>
                  </div>
                )}

                {uploadedMedia.length > 0 && (
                  <div>
                    <h4 className="text-sm font-medium mb-3">
                      Uploaded Media ({uploadedMedia.length})
                    </h4>
                    <div className="grid grid-cols-4 gap-4">
                      {uploadedMedia.map((media) => (
                        <div
                          key={media.id}
                          className="relative aspect-square rounded-lg overflow-hidden border group"
                        >
                          <div className="w-full h-full bg-gray-100 flex items-center justify-center">
                            <ImageIcon className="h-8 w-8 text-gray-400" />
                          </div>
                          <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                            <Button
                              type="button"
                              variant="destructive"
                              size="sm"
                              onClick={() => {
                                setUploadedMedia((prev) =>
                                  prev.filter((m) => m.id !== media.id)
                                );
                              }}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                          <div className="absolute bottom-0 left-0 right-0 bg-black/60 text-white p-2">
                            <p className="text-xs truncate">{media.originalFileName}</p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          )}

          <div className="flex justify-between">
            <Button type="button" variant="outline" onClick={prevStep} disabled={currentStep === 1}>
              <ChevronLeft className="mr-2 h-4 w-4" />
              Previous
            </Button>

            {currentStep === STEPS.length ? (
              <Button type="submit" disabled={createMutation.isPending}>
                <Save className="mr-2 h-4 w-4" />
                {createMutation.isPending ? "Creating..." : "Create Listing"}
              </Button>
            ) : (
              <Button type="button" onClick={nextStep}>
                Next
                <ChevronRight className="ml-2 h-4 w-4" />
              </Button>
            )}
          </div>
        </form>
      </Form>
    </div>
  );
}
