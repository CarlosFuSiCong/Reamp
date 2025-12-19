import Link from "next/link";
import Image from "next/image";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Bed, Bath, Car, Maximize, MapPin, ArrowUpRight } from "lucide-react";
import type { Listing } from "@/types";
import { ListingType } from "@/types";
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
    <Link href={`/listings/${listing.id}`} className="group block h-full">
      <Card className="overflow-hidden bg-white border-gray-100 hover:border-blue-200 shadow-sm hover:shadow-xl hover:shadow-blue-900/5 transition-all duration-300 h-full flex flex-col rounded-xl">
        {/* Image */}
        <div className="relative aspect-[4/3] overflow-hidden bg-gray-100">
          <Image
            src={imageUrl}
            alt={listing.title}
            fill
            className="object-cover group-hover:scale-105 transition-transform duration-500"
            sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
          />
          
          {/* Overlay Gradient */}
          <div className="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-transparent opacity-60"></div>
          
          {/* Badges */}
          <div className="absolute top-3 left-3 flex flex-wrap gap-2">
            <Badge className="bg-white/95 text-gray-900 hover:bg-white backdrop-blur-sm shadow-sm font-medium border-0">
              {getPropertyTypeLabel(listing.propertyType)}
            </Badge>
          </div>
          
          <div className="absolute top-3 right-3">
            <Badge
              variant={listing.listingType === ListingType.ForSale ? "default" : "secondary"}
              className={`${
                listing.listingType === ListingType.ForSale 
                  ? "bg-gray-900 text-white hover:bg-gray-800" 
                  : "bg-green-500 text-white hover:bg-green-600"
              } shadow-sm border-0`}
            >
              {getListingTypeLabel(listing.listingType)}
            </Badge>
        </div>

          <div className="absolute bottom-3 left-3 text-white">
             <p className="font-bold text-xl drop-shadow-md">
              {formatPrice(listing.price, listing.currency)}
              {listing.listingType === ListingType.ForRent && (
                <span className="text-sm font-normal opacity-90 ml-1">/mo</span>
              )}
            </p>
          </div>
          </div>

        <CardContent className="p-5 flex flex-col flex-grow">
          {/* Title & Location */}
          <div className="mb-4 flex-grow">
            <div className="flex justify-between items-start gap-2 mb-2">
              <h3 className="text-lg font-bold text-gray-900 line-clamp-1 group-hover:text-blue-600 transition-colors">
            {listing.title}
          </h3>
              <ArrowUpRight className="h-5 w-5 text-gray-300 group-hover:text-blue-600 transition-colors flex-shrink-0 opacity-0 group-hover:opacity-100" />
            </div>
            
            <div className="flex items-start gap-1.5 text-sm text-gray-500">
              <MapPin className="h-4 w-4 mt-0.5 flex-shrink-0 text-gray-400" />
              <span className="line-clamp-1 font-medium">
              {listing.city}, {listing.state}
            </span>
          </div>
          </div>

          <Separator className="mb-4 bg-gray-50" />

          {/* Features */}
          <div className="flex items-center justify-between text-sm text-gray-600">
            <div className="flex items-center gap-4">
            {listing.bedrooms > 0 && (
                <div className="flex items-center gap-1.5" title={`${listing.bedrooms} Bedrooms`}>
                  <Bed className="h-4 w-4 text-gray-400" />
                  <span className="font-semibold">{listing.bedrooms}</span>
              </div>
            )}
            {listing.bathrooms > 0 && (
                <div className="flex items-center gap-1.5" title={`${listing.bathrooms} Bathrooms`}>
                  <Bath className="h-4 w-4 text-gray-400" />
                  <span className="font-semibold">{listing.bathrooms}</span>
              </div>
            )}
            {listing.parkingSpaces > 0 && (
                <div className="flex items-center gap-1.5" title={`${listing.parkingSpaces} Parking Spaces`}>
                  <Car className="h-4 w-4 text-gray-400" />
                  <span className="font-semibold">{listing.parkingSpaces}</span>
              </div>
            )}
            </div>
            
            {(listing.floorAreaSqm || listing.landAreaSqm) && (
              <div className="flex items-center gap-1.5 text-gray-500" title="Area">
                <Maximize className="h-3.5 w-3.5" />
                <span>{listing.floorAreaSqm || listing.landAreaSqm} m²</span>
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

function Separator({ className }: { className?: string }) {
  return <div className={`h-px w-full ${className}`} />;
}
