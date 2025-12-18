"use client";

import { APIProvider } from "@vis.gl/react-google-maps";
import { ReactNode } from "react";

interface GoogleMapsProviderProps {
  children: ReactNode;
}

export function GoogleMapsProvider({ children }: GoogleMapsProviderProps) {
  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    console.warn("Google Maps API key is not configured. Map features will be disabled.");
    return <>{children}</>;
  }

  return (
    <APIProvider apiKey={apiKey} libraries={["places", "geocoding"]}>
      {children}
    </APIProvider>
  );
}
