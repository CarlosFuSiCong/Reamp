"use client";

import { useState } from "react";
import { UseFormReturn } from "react-hook-form";
import { AddressAutocomplete, AddressComponents } from "@/components/maps";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { MapPin } from "lucide-react";

interface ListingAddressFieldsProps {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  form: UseFormReturn<any>;
}

export function ListingAddressFields({ form }: ListingAddressFieldsProps) {
  const [useAutocomplete, setUseAutocomplete] = useState(true);

  const handleAddressSelect = (fullAddress: string, components?: AddressComponents) => {
    if (components) {
      form.setValue("addressLine1", components.line1, { shouldValidate: true });
      form.setValue("addressLine2", components.line2 || "", { shouldValidate: true });
      form.setValue("city", components.city, { shouldValidate: true });
      form.setValue("state", components.state, { shouldValidate: true });
      form.setValue("postcode", components.postcode, { shouldValidate: true });
      form.setValue("country", components.country, { shouldValidate: true });

      // Store coordinates if available
      if (components.latitude && components.longitude) {
        form.setValue("latitude", components.latitude);
        form.setValue("longitude", components.longitude);
      }
    }
  };

  if (!useAutocomplete || !process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY) {
    return (
      <div className="space-y-4">
        <div className="flex justify-between items-center">
          <p className="text-sm text-muted-foreground">Manual Address Entry</p>
          {process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY && (
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => setUseAutocomplete(true)}
            >
              <MapPin className="h-4 w-4 mr-2" />
              Use Address Search
            </Button>
          )}
        </div>

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
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <FormLabel>Address Search</FormLabel>
        <Button type="button" variant="ghost" size="sm" onClick={() => setUseAutocomplete(false)}>
          Enter Manually
        </Button>
      </div>

      <AddressAutocomplete
        value=""
        onChange={handleAddressSelect}
        placeholder="Start typing an Australian address... (e.g., 1 George Street, Sydney)"
      />

      <div className="space-y-4 pt-4">
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
      </div>
    </div>
  );
}
