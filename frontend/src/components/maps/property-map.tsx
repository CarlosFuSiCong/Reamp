"use client";

import { Map, Marker } from "@vis.gl/react-google-maps";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { MapPin } from "lucide-react";

interface PropertyMapProps {
  latitude?: number;
  longitude?: number;
  address?: string;
  title?: string;
  zoom?: number;
  height?: string;
}

export function PropertyMap({
  latitude,
  longitude,
  address,
  title = "Property Location",
  zoom = 15,
  height = "400px",
}: PropertyMapProps) {
  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>{title}</CardTitle>
          {address && <CardDescription>{address}</CardDescription>}
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center h-64 bg-muted rounded-lg">
            <div className="text-center text-muted-foreground">
              <MapPin className="h-12 w-12 mx-auto mb-2 opacity-50" />
              <p>Map is not available</p>
              <p className="text-sm mt-1">Google Maps API key not configured</p>
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!latitude || !longitude) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>{title}</CardTitle>
          {address && <CardDescription>{address}</CardDescription>}
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center h-64 bg-muted rounded-lg">
            <div className="text-center text-muted-foreground">
              <MapPin className="h-12 w-12 mx-auto mb-2 opacity-50" />
              <p>Location coordinates not available</p>
              {address && <p className="text-sm mt-1">{address}</p>}
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  const center = { lat: latitude, lng: longitude };

  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        {address && <CardDescription>{address}</CardDescription>}
      </CardHeader>
      <CardContent>
        <div style={{ height }} className="rounded-lg overflow-hidden border">
          <Map
            defaultCenter={center}
            defaultZoom={zoom}
            gestureHandling="greedy"
            disableDefaultUI={false}
            mapId={process.env.NEXT_PUBLIC_GOOGLE_MAPS_MAP_ID || "DEMO_MAP_ID"}
          >
            <Marker position={center} title={address} />
          </Map>
        </div>
      </CardContent>
    </Card>
  );
}
