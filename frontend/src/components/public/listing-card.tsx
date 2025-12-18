import Link from "next/link";
import Image from "next/image";
import { Card, CardContent, CardFooter } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Bed, Bath, Car, Maximize, MapPin } from "lucide-react";
import type { Listing } from "@/types";
import { getPropertyTypeLabel, getListingTypeLabel } from "@/lib/utils/enum-labels";

interface ListingCardProps {
  listing: Listing;
}

export function ListingCard({ listing }: ListingCardProps) {
  // Get cover image or first image
  const coverImage = listing.media?.find((m) => m.isCover) || listing.media?.[0];
  const imageUrl = coverImage?.thumbnailUrl || "/placeholder-property.jpg";

  const formatPrice = (price: number, currency: string) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: currency || "USD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(price);
  };

  return (
    <Link href={`/listings/${listing.id}`}>
      <Card className="overflow-hidden hover:shadow-lg transition-all duration-300 group cursor-pointer h-full">
        {/* Image */}
        <div className="relative aspect-[4/3] overflow-hidden bg-gray-100">
          <Image
            src={imageUrl}
            alt={listing.title}
            fill
            className="object-cover group-hover:scale-110 transition-transform duration-300"
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
          />
          {/* Property Type Badge */}
          <div className="absolute top-3 left-3">
            <Badge className="bg-white/90 text-gray-900 hover:bg-white">
              {getPropertyTypeLabel(listing.propertyType)}
            </Badge>
          </div>
          {/* Listing Type Badge */}
          <div className="absolute top-3 right-3">
            <Badge
              variant={listing.listingType === "Sale" ? "default" : "secondary"}
              className="bg-blue-600/90 text-white hover:bg-blue-600"
            >
              {getListingTypeLabel(listing.listingType)}
            </Badge>
          </div>
        </div>

        <CardContent className="p-4">
          {/* Price */}
          <div className="mb-2">
            <p className="text-2xl font-bold text-gray-900">
              {formatPrice(listing.price, listing.currency)}
              {listing.listingType === "Rent" && (
                <span className="text-base font-normal text-gray-500">/month</span>
              )}
            </p>
          </div>

          {/* Title */}
          <h3 className="text-lg font-semibold text-gray-900 mb-2 line-clamp-2 group-hover:text-blue-600 transition-colors">
            {listing.title}
          </h3>

          {/* Location */}
          <div className="flex items-start gap-1.5 text-sm text-gray-600 mb-3">
            <MapPin className="h-4 w-4 mt-0.5 flex-shrink-0" />
            <span className="line-clamp-1">
              {listing.city}, {listing.state}
            </span>
          </div>

          {/* Features */}
          <div className="flex items-center gap-4 text-sm text-gray-600">
            {listing.bedrooms > 0 && (
              <div className="flex items-center gap-1.5">
                <Bed className="h-4 w-4" />
                <span>{listing.bedrooms}</span>
              </div>
            )}
            {listing.bathrooms > 0 && (
              <div className="flex items-center gap-1.5">
                <Bath className="h-4 w-4" />
                <span>{listing.bathrooms}</span>
              </div>
            )}
            {listing.parkingSpaces > 0 && (
              <div className="flex items-center gap-1.5">
                <Car className="h-4 w-4" />
                <span>{listing.parkingSpaces}</span>
              </div>
            )}
            {listing.floorAreaSqm && (
              <div className="flex items-center gap-1.5">
                <Maximize className="h-4 w-4" />
                <span>{listing.floorAreaSqm} m²</span>
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

