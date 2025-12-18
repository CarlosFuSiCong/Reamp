"use client";

import { useEffect, useRef, useState } from "react";
import { Input } from "@/components/ui/input";
import { FormControl, FormMessage } from "@/components/ui/form";
import { useMapsLibrary } from "@vis.gl/react-google-maps";
import { MapPin } from "lucide-react";

// Type declaration for Google Maps
declare global {
  interface Window {
    google?: typeof google;
  }
  namespace google {
    namespace maps {
      namespace places {
        class Autocomplete {
          constructor(input: HTMLInputElement, opts?: any);
          addListener(event: string, handler: () => void): void;
          getPlace(): any;
        }
      }
    }
  }
}

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
  const [isApiReady, setIsApiReady] = useState(false);
  const [inputValue, setInputValue] = useState(value);
  
  const places = useMapsLibrary("places");

  useEffect(() => {
    setInputValue(value);
  }, [value]);

  useEffect(() => {
    if (!places || !inputRef.current) {
      return;
    }

    setIsApiReady(true);

    try {
      autocompleteRef.current = new places.Autocomplete(inputRef.current, {
        types: ["address"],
        componentRestrictions: { country: "au" },
        fields: ["address_components", "formatted_address", "geometry"],
      });

      if (autocompleteRef.current) {
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

        const formattedAddress = place.formatted_address || "";
        setInputValue(formattedAddress);
        onChange(formattedAddress, components);
        });
      }
    } catch (error) {
      console.error("Failed to initialize Google Places Autocomplete:", error);
    }

    return () => {
      if (autocompleteRef.current) {
        google.maps.event.clearInstanceListeners(autocompleteRef.current);
      }
    };
  }, [places, onChange]);

  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setInputValue(newValue);
    onChange(newValue);
  };

  if (!apiKey) {
    return (
      <FormControl>
        <Input
          type="text"
          value={inputValue}
          onChange={handleInputChange}
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
            value={inputValue}
            onChange={handleInputChange}
            placeholder={placeholder}
            disabled={disabled}
            className="pl-10"
            autoComplete="off"
          />
        </div>
      </FormControl>
      {error && <FormMessage>{error}</FormMessage>}
      {!isApiReady && (
        <p className="text-xs text-muted-foreground mt-1">Loading address suggestions...</p>
      )}
    </div>
  );
}
