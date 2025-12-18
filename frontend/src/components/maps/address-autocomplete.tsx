"use client";

import { useEffect, useRef, useState } from "react";
import { Input } from "@/components/ui/input";
import { FormControl, FormMessage } from "@/components/ui/form";
import { importLibrary } from "@googlemaps/js-api-loader";
import { MapPin } from "lucide-react";

export interface AddressComponents {
  line1: string;
  line2?: string;
  city: string;
  state: string;
  postcode: string;
  country: string;
  latitude?: number;
  longitude?: number;
}

interface AddressAutocompleteProps {
  value: string;
  onChange: (value: string, components?: AddressComponents) => void;
  placeholder?: string;
  disabled?: boolean;
  error?: string;
}

export function AddressAutocomplete({
  value,
  onChange,
  placeholder = "Enter an address...",
  disabled = false,
  error,
}: AddressAutocompleteProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const autocompleteRef = useRef<google.maps.places.Autocomplete | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isApiAvailable, setIsApiAvailable] = useState(true);

  useEffect(() => {
    const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

    if (!apiKey || !inputRef.current) {
      return;
    }

    setIsLoading(true);

    // 使用新的函数式 API
    importLibrary("places")
      .then((placesLibrary) => {
        if (!inputRef.current) return;

        // @ts-ignore - Google Maps types
        autocompleteRef.current = new placesLibrary.Autocomplete(inputRef.current, {
          types: ["address"],
          componentRestrictions: { country: "au" }, // 仅限澳大利亚地址
          fields: ["address_components", "formatted_address", "geometry"],
        });

        autocompleteRef.current.addListener("place_changed", () => {
          const place = autocompleteRef.current?.getPlace();

          if (!place || !place.address_components) {
            return;
          }

          const components: AddressComponents = {
            line1: "",
            line2: "",
            city: "",
            state: "",
            postcode: "",
            country: "",
            latitude: place.geometry?.location?.lat(),
            longitude: place.geometry?.location?.lng(),
          };

          let streetNumber = "";
          let route = "";

          place.address_components.forEach((component) => {
            const types = component.types;

            if (types.includes("street_number")) {
              streetNumber = component.long_name;
            } else if (types.includes("route")) {
              route = component.long_name;
            } else if (types.includes("subpremise")) {
              components.line2 = component.long_name;
            } else if (types.includes("locality")) {
              components.city = component.long_name;
            } else if (types.includes("administrative_area_level_1")) {
              components.state = component.short_name;
            } else if (types.includes("postal_code")) {
              components.postcode = component.long_name;
            } else if (types.includes("country")) {
              components.country = component.long_name;
            }
          });

          components.line1 = [streetNumber, route].filter(Boolean).join(" ");

          onChange(place.formatted_address || value, components);
        });

        setIsLoading(false);
      })
      .catch((error) => {
        console.error("Failed to load Google Maps API:", error);
        setIsApiAvailable(false);
        setIsLoading(false);
      });

    return () => {
      if (autocompleteRef.current) {
        google.maps.event.clearInstanceListeners(autocompleteRef.current);
      }
    };
  }, []);

  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  if (!apiKey || !isApiAvailable) {
    return (
      <FormControl>
        <Input
          type="text"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          placeholder={placeholder}
          disabled={disabled}
        />
      </FormControl>
    );
  }

  return (
    <div className="relative">
      <FormControl>
        <div className="relative">
          <MapPin className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            ref={inputRef}
            type="text"
            value={value}
            onChange={(e) => onChange(e.target.value)}
            placeholder={placeholder}
            disabled={disabled || isLoading}
            className="pl-10"
          />
        </div>
      </FormControl>
      {error && <FormMessage>{error}</FormMessage>}
      {isLoading && (
        <p className="text-xs text-muted-foreground mt-1">Loading address suggestions...</p>
      )}
    </div>
  );
}
