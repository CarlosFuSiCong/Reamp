import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { listingsApi } from "@/lib/api";
import { ImageGallery } from "@/components/public";
import { Footer } from "@/components/layout";

import { Navbar } from "@/components/layout";
import { PropertyMap } from "@/components/maps";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ListingType } from "@/types";
import { Separator } from "@/components/ui/separator";
import {
  Bed,
  Bath,
  Car,
  Maximize,
  MapPin,
  Mail,
  Phone,
  User,
  Share2,
  Heart,
  ChevronLeft,
} from "lucide-react";
import { getPropertyTypeLabel, getListingTypeLabel } from "@/lib/utils/enum-labels";
import Link from "next/link";

// Generate metadata for SEO
export async function generateMetadata({ params }: { params: Promise<{ id: string }> }): Promise<Metadata> {
  const { id } = await params;
  try {
    const listing = await listingsApi.getByIdPublic(id);
    const coverImage = listing.media?.find((m) => m.isCover) || listing.media?.[0];
    const imageUrl = coverImage?.thumbnailUrl;

    return {
      title: `${listing.title} - Reamp`,
      description: listing.description || `${getPropertyTypeLabel(listing.propertyType)} for ${getListingTypeLabel(listing.listingType)} in ${listing.city}`,
      openGraph: {
        title: listing.title,
        description: listing.description,
        images: imageUrl ? [{ url: imageUrl }] : [],
        type: "website",
      },
      twitter: {
        card: "summary_large_image",
        title: listing.title,
        description: listing.description,
        images: imageUrl ? [imageUrl] : [],
      },
    };
  } catch {
    return {
      title: "Property Not Found - Reamp",
    };
  }
}

export default async function ListingDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  let listing;
  
  try {
    listing = await listingsApi.getByIdPublic(id);
  } catch {
    notFound();
  }

  const formatPrice = (price: number, currency: string) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: currency || "USD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(price);
  };

  const primaryAgent = listing.agents?.find((a) => a.isPrimary) || listing.agents?.[0];

  return (
    <div className="min-h-screen flex flex-col bg-gray-50/30">
      <Navbar />

      <main className="flex-1 pb-16">
        {/* Breadcrumb / Back Navigation */}
        <div className="bg-white border-b border-gray-100 sticky top-[64px] z-30 shadow-sm backdrop-blur-md bg-white/90">
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 h-14 flex items-center justify-between">
            <Link 
              href="/listings" 
              className="flex items-center text-sm font-medium text-gray-500 hover:text-blue-600 transition-colors"
            >
              <ChevronLeft className="h-4 w-4 mr-1" />
              Back to listings
            </Link>
            <div className="flex gap-2">
               <Button variant="ghost" size="sm" className="text-gray-500 gap-2 hover:text-blue-600 hover:bg-blue-50">
                 <Share2 className="h-4 w-4" />
                 Share
               </Button>
               <Button variant="ghost" size="sm" className="text-gray-500 gap-2 hover:text-red-600 hover:bg-red-50">
                 <Heart className="h-4 w-4" />
                 Save
               </Button>
            </div>
          </div>
        </div>

        <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 xl:gap-12">
            {/* Main Content */}
            <div className="lg:col-span-2 space-y-8">
              {/* Gallery */}
              <div className="rounded-2xl overflow-hidden shadow-lg border border-gray-100 bg-white">
              <ImageGallery media={listing.media || []} title={listing.title} />
              </div>

              {/* Title and Price */}
              <div className="space-y-6">
                <div className="flex flex-col md:flex-row md:justify-between md:items-start gap-4">
                  <div className="space-y-2 flex-1">
                    <div className="flex flex-wrap gap-2 mb-3">
                      <Badge variant="secondary" className="bg-blue-50 text-blue-700 hover:bg-blue-100 px-3 py-1 text-sm font-medium">
                        {getPropertyTypeLabel(listing.propertyType)}
                      </Badge>
                      <Badge
                        variant={listing.listingType === ListingType.ForSale ? "default" : "secondary"}
                        className={`text-sm font-medium px-3 py-1 ${
                          listing.listingType === ListingType.ForSale 
                            ? "bg-gray-900 hover:bg-gray-800" 
                            : "bg-green-100 text-green-800 hover:bg-green-200 border-green-200"
                        }`}
                      >
                        For {getListingTypeLabel(listing.listingType)}
                      </Badge>
                    </div>
                    <h1 className="text-3xl md:text-4xl font-bold text-gray-900 tracking-tight leading-tight">
                      {listing.title}
                    </h1>
                    <div className="flex items-center gap-2 text-gray-500 text-lg">
                      <MapPin className="h-5 w-5 text-gray-400 flex-shrink-0" />
                      <span>
                        {listing.addressLine1}, {listing.city}, {listing.state} {listing.postcode}
                      </span>
                    </div>
                  </div>
                  <div className="text-left md:text-right shrink-0">
                    <p className="text-3xl md:text-4xl font-bold text-blue-600 tracking-tight">
                      {formatPrice(listing.price, listing.currency)}
                      {listing.listingType === ListingType.ForRent && (
                        <span className="text-lg font-medium text-gray-500 ml-1">/mo</span>
                      )}
                    </p>
                    <p className="text-gray-400 text-sm mt-1 font-medium">Price guide</p>
                  </div>
                </div>

                {/* Key Features Grid */}
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                  <div className="flex flex-col items-center justify-center p-6 bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition-all group">
                    <Bed className="h-7 w-7 text-gray-400 group-hover:text-blue-600 transition-colors mb-3" />
                    <span className="text-2xl font-bold text-gray-900">{listing.bedrooms}</span>
                    <span className="text-sm text-gray-500 font-medium">Bedrooms</span>
                </div>
                  <div className="flex flex-col items-center justify-center p-6 bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition-all group">
                    <Bath className="h-7 w-7 text-gray-400 group-hover:text-blue-600 transition-colors mb-3" />
                    <span className="text-2xl font-bold text-gray-900">{listing.bathrooms}</span>
                    <span className="text-sm text-gray-500 font-medium">Bathrooms</span>
              </div>
                  <div className="flex flex-col items-center justify-center p-6 bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition-all group">
                    <Car className="h-7 w-7 text-gray-400 group-hover:text-blue-600 transition-colors mb-3" />
                    <span className="text-2xl font-bold text-gray-900">{listing.parkingSpaces}</span>
                    <span className="text-sm text-gray-500 font-medium">Parking</span>
                      </div>
                  <div className="flex flex-col items-center justify-center p-6 bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition-all group">
                    <Maximize className="h-7 w-7 text-gray-400 group-hover:text-blue-600 transition-colors mb-3" />
                    <span className="text-2xl font-bold text-gray-900">
                      {listing.floorAreaSqm || listing.landAreaSqm || "-"}
                    </span>
                    <span className="text-sm text-gray-500 font-medium">Area m²</span>
                      </div>
                      </div>
                  </div>

              <Separator className="bg-gray-200" />

              {/* Description */}
              <div className="space-y-4">
                <h2 className="text-2xl font-bold text-gray-900">About this property</h2>
                <div className="prose prose-lg prose-blue max-w-none text-gray-600 leading-relaxed whitespace-pre-line">
                  {listing.description || "No description provided."}
                    </div>
                  </div>

              <Separator className="bg-gray-200" />

              {/* Location */}
              <div className="space-y-4">
                <h2 className="text-2xl font-bold text-gray-900">Location</h2>
                
                {/* Map */}
                {listing.latitude && listing.longitude ? (
                  <div className="rounded-2xl overflow-hidden border border-gray-200 shadow-sm">
                    <PropertyMap
                      latitude={listing.latitude}
                      longitude={listing.longitude}
                      address={`${listing.addressLine1}, ${listing.city}, ${listing.state}`}
                    />
                  </div>
                ) : (
                  <div className="bg-gray-50 p-6 rounded-2xl border border-gray-200">
                    <div className="space-y-2 text-gray-700">
                      <p className="font-medium">{listing.addressLine1}</p>
                      {listing.addressLine2 && <p>{listing.addressLine2}</p>}
                      <p>{listing.city}, {listing.state} {listing.postcode}</p>
                      <p>{listing.country}</p>
                    </div>
                  </div>
                )}
              </div>
            </div>

            {/* Sidebar */}
            <div className="space-y-6">
              {/* Contact Agent Card */}
              {primaryAgent && (
                <div className="sticky top-[140px] space-y-6">
                  <Card className="shadow-xl border-blue-100 overflow-hidden bg-white/80 backdrop-blur-sm">
                    <div className="bg-gradient-to-r from-blue-600 to-blue-500 h-2 w-full"></div>
                    <CardHeader className="pb-4">
                      <CardTitle className="text-xl font-bold text-gray-900">Contact Agent</CardTitle>
                  </CardHeader>
                    <CardContent className="space-y-6">
                      <div className="flex items-center gap-4">
                        <div className="w-16 h-16 bg-gradient-to-br from-gray-100 to-gray-200 rounded-full flex items-center justify-center border-2 border-white shadow-md overflow-hidden">
                          <User className="h-8 w-8 text-gray-400" />
                      </div>
                      <div>
                          <p className="font-bold text-lg text-gray-900">
                          {primaryAgent.firstName} {primaryAgent.lastName}
                        </p>
                        {primaryAgent.isPrimary && (
                            <Badge variant="secondary" className="text-xs bg-blue-50 text-blue-700 mt-1 font-medium border-blue-100">
                              Listing Agent
                          </Badge>
                        )}
                      </div>
                    </div>

                      <div className="space-y-3 pt-2">
                      {primaryAgent.email && (
                          <a
                            href={`mailto:${primaryAgent.email}`}
                            className="flex items-center gap-3 text-sm p-3 rounded-lg hover:bg-blue-50 transition-all border border-transparent hover:border-blue-100 group"
                          >
                            <div className="bg-gray-100 p-2 rounded-full group-hover:bg-blue-100 transition-colors">
                              <Mail className="h-4 w-4 text-gray-600 group-hover:text-blue-600" />
                            </div>
                            <span className="text-gray-600 group-hover:text-blue-700 font-medium truncate">{primaryAgent.email}</span>
                          </a>
                      )}
                      {primaryAgent.phone && (
                          <a
                            href={`tel:${primaryAgent.phone}`}
                            className="flex items-center gap-3 text-sm p-3 rounded-lg hover:bg-blue-50 transition-all border border-transparent hover:border-blue-100 group"
                          >
                            <div className="bg-gray-100 p-2 rounded-full group-hover:bg-blue-100 transition-colors">
                              <Phone className="h-4 w-4 text-gray-600 group-hover:text-blue-600" />
                            </div>
                            <span className="text-gray-600 group-hover:text-blue-700 font-medium">{primaryAgent.phone}</span>
                          </a>
                      )}
                    </div>

                      <Button className="w-full h-12 text-base font-semibold shadow-lg shadow-blue-600/20 bg-blue-600 hover:bg-blue-700 transition-all hover:scale-[1.02]">
                        Enquire Now
                    </Button>
                  </CardContent>
                </Card>

              {/* Additional Agents */}
              {listing.agents && listing.agents.length > 1 && (
                    <Card className="border-gray-100 shadow-sm">
                      <CardHeader className="pb-2">
                        <CardTitle className="text-sm font-bold text-gray-500 uppercase tracking-wider">Other Agents</CardTitle>
                  </CardHeader>
                  <CardContent>
                        <div className="space-y-1">
                      {listing.agents
                        .filter((a) => !a.isPrimary)
                        .map((agent, index) => (
                              <div key={index} className="flex items-center gap-3 text-sm p-2 rounded-lg hover:bg-gray-50 transition-colors">
                                <div className="w-8 h-8 bg-gray-100 rounded-full flex items-center justify-center flex-shrink-0 text-gray-400">
                                  <User className="h-4 w-4" />
                            </div>
                            <div className="flex-1 min-w-0">
                                  <p className="font-medium text-gray-900 truncate">
                                {agent.firstName} {agent.lastName}
                              </p>
                            </div>
                          </div>
                        ))}
                    </div>
                  </CardContent>
                </Card>
              )}

                  <div className="text-center">
                    <p className="text-xs text-gray-400 font-mono">
                      ID: {listing.id.slice(0, 8).toUpperCase()}
                    </p>
                    </div>
                  </div>
              )}
            </div>
          </div>
        </div>
      </main>

      <Footer />

      {/* JSON-LD Structured Data for SEO */}
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{
          __html: JSON.stringify({
            "@context": "https://schema.org",
            "@type": listing.listingType === ListingType.ForSale ? "RealEstateListing" : "Apartment",
            name: listing.title,
            description: listing.description,
            image: listing.media?.map((m) => m.thumbnailUrl).filter(Boolean),
            address: {
              "@type": "PostalAddress",
              streetAddress: listing.addressLine1,
              addressLocality: listing.city,
              addressRegion: listing.state,
              postalCode: listing.postcode,
              addressCountry: listing.country,
            },
            offers: {
              "@type": "Offer",
              price: listing.price,
              priceCurrency: listing.currency || "USD",
            },
            numberOfRooms: listing.bedrooms,
            numberOfBathroomsTotal: listing.bathrooms,
            floorSize: {
              "@type": "QuantitativeValue",
              value: listing.floorAreaSqm,
              unitCode: "MTK",
            },
          }),
        }}
      />
    </div>
  );
}
