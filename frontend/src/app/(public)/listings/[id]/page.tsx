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
  Calendar,
  Building2,
} from "lucide-react";
import { getPropertyTypeLabel, getListingTypeLabel } from "@/lib/utils/enum-labels";

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
    <div className="min-h-screen flex flex-col">
      <Navbar />

      <main className="flex-1">
        <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Main Content */}
            <div className="lg:col-span-2 space-y-6">
              {/* Gallery */}
              <ImageGallery media={listing.media || []} title={listing.title} />

              {/* Title and Price */}
              <div>
                <div className="flex flex-wrap items-start justify-between gap-4 mb-4">
                  <div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">{listing.title}</h1>
                    <div className="flex items-center gap-2 text-gray-600">
                      <MapPin className="h-5 w-5" />
                      <span>
                        {listing.addressLine1}, {listing.city}, {listing.state} {listing.postcode}
                      </span>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-3xl font-bold text-blue-600">
                      {formatPrice(listing.price, listing.currency)}
                      {listing.listingType === "Rent" && (
                        <span className="text-lg font-normal text-gray-500">/month</span>
                      )}
                    </p>
                  </div>
                </div>

                {/* Badges */}
                <div className="flex flex-wrap gap-2">
                  <Badge variant="secondary">{getPropertyTypeLabel(listing.propertyType)}</Badge>
                  <Badge
                    variant={listing.listingType === "Sale" ? "default" : "secondary"}
                    className="bg-blue-600 text-white"
                  >
                    For {getListingTypeLabel(listing.listingType)}
                  </Badge>
                </div>
              </div>

              <Separator />

              {/* Key Features */}
              <Card>
                <CardHeader>
                  <CardTitle>Property Features</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 sm:grid-cols-4 gap-6">
                    {listing.bedrooms > 0 && (
                      <div className="flex flex-col items-center text-center p-4 bg-gray-50 rounded-lg">
                        <Bed className="h-8 w-8 text-blue-600 mb-2" />
                        <p className="text-2xl font-bold">{listing.bedrooms}</p>
                        <p className="text-sm text-gray-600">Bedrooms</p>
                      </div>
                    )}
                    {listing.bathrooms > 0 && (
                      <div className="flex flex-col items-center text-center p-4 bg-gray-50 rounded-lg">
                        <Bath className="h-8 w-8 text-blue-600 mb-2" />
                        <p className="text-2xl font-bold">{listing.bathrooms}</p>
                        <p className="text-sm text-gray-600">Bathrooms</p>
                      </div>
                    )}
                    {listing.parkingSpaces > 0 && (
                      <div className="flex flex-col items-center text-center p-4 bg-gray-50 rounded-lg">
                        <Car className="h-8 w-8 text-blue-600 mb-2" />
                        <p className="text-2xl font-bold">{listing.parkingSpaces}</p>
                        <p className="text-sm text-gray-600">Parking</p>
                      </div>
                    )}
                    {listing.floorAreaSqm && (
                      <div className="flex flex-col items-center text-center p-4 bg-gray-50 rounded-lg">
                        <Maximize className="h-8 w-8 text-blue-600 mb-2" />
                        <p className="text-2xl font-bold">{listing.floorAreaSqm}</p>
                        <p className="text-sm text-gray-600">m² Floor Area</p>
                      </div>
                    )}
                  </div>

                  {listing.landAreaSqm && (
                    <div className="mt-4 p-4 bg-blue-50 rounded-lg">
                      <p className="text-sm text-gray-600">Land Area</p>
                      <p className="text-xl font-bold text-blue-600">{listing.landAreaSqm} m²</p>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Description */}
              {listing.description && (
                <Card>
                  <CardHeader>
                    <CardTitle>About This Property</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <p className="text-gray-700 whitespace-pre-line leading-relaxed">
                      {listing.description}
                    </p>
                  </CardContent>
                </Card>
              )}

              {/* Address Details */}
              <Card>
                <CardHeader>
                  <CardTitle>Location</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="flex items-center gap-3 flex-wrap">
                    <MapPin className="h-5 w-5 text-blue-600 flex-shrink-0" />
                    <div className="flex items-center gap-2 flex-wrap text-gray-700">
                      <span className="font-semibold">{listing.addressLine1}</span>
                      {listing.addressLine2 && (
                        <>
                          <span className="text-gray-400">·</span>
                          <span>{listing.addressLine2}</span>
                        </>
                      )}
                      <span className="text-gray-400">·</span>
                      <span>{listing.city}, {listing.state} {listing.postcode}</span>
                      <span className="text-gray-400">·</span>
                      <span className="text-gray-500">
                        {listing.country === 'AU' ? 'Australia' : listing.country}
                      </span>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Property Map */}
              <PropertyMap
                latitude={listing.latitude}
                longitude={listing.longitude}
                address={`${listing.addressLine1}, ${listing.city}, ${listing.state} ${listing.postcode}`}
                title="Property Location"
              />
            </div>

            {/* Sidebar */}
            <div className="space-y-6">
              {/* Contact Agent Card */}
              {primaryAgent && (
                <Card className="sticky top-4">
                  <CardHeader>
                    <CardTitle>Contact Agent</CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div className="flex items-center gap-3">
                      <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
                        <User className="h-6 w-6 text-blue-600" />
                      </div>
                      <div>
                        <p className="font-semibold">
                          {primaryAgent.firstName} {primaryAgent.lastName}
                        </p>
                        {primaryAgent.isPrimary && (
                          <Badge variant="secondary" className="text-xs">
                            Primary Agent
                          </Badge>
                        )}
                      </div>
                    </div>

                    <Separator />

                    <div className="space-y-3">
                      {primaryAgent.email && (
                        <div className="flex items-center gap-2 text-sm">
                          <Mail className="h-4 w-4 text-gray-400" />
                          <a
                            href={`mailto:${primaryAgent.email}`}
                            className="text-blue-600 hover:underline"
                          >
                            {primaryAgent.email}
                          </a>
                        </div>
                      )}
                      {primaryAgent.phone && (
                        <div className="flex items-center gap-2 text-sm">
                          <Phone className="h-4 w-4 text-gray-400" />
                          <a
                            href={`tel:${primaryAgent.phone}`}
                            className="text-blue-600 hover:underline"
                          >
                            {primaryAgent.phone}
                          </a>
                        </div>
                      )}
                    </div>

                    <Button className="w-full gap-2">
                      <Mail className="h-4 w-4" />
                      Send Message
                    </Button>
                  </CardContent>
                </Card>
              )}

              {/* Additional Agents */}
              {listing.agents && listing.agents.length > 1 && (
                <Card>
                  <CardHeader>
                    <CardTitle className="text-lg">Other Agents</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="space-y-3">
                      {listing.agents
                        .filter((a) => !a.isPrimary)
                        .map((agent, index) => (
                          <div key={index} className="flex items-center gap-3 text-sm">
                            <div className="w-10 h-10 bg-gray-100 rounded-full flex items-center justify-center flex-shrink-0">
                              <User className="h-5 w-5 text-gray-400" />
                            </div>
                            <div className="flex-1 min-w-0">
                              <p className="font-medium truncate">
                                {agent.firstName} {agent.lastName}
                              </p>
                              {agent.email && (
                                <p className="text-gray-500 text-xs truncate">{agent.email}</p>
                              )}
                            </div>
                          </div>
                        ))}
                    </div>
                  </CardContent>
                </Card>
              )}

              {/* Property ID */}
              <Card>
                <CardContent className="pt-6">
                  <div className="space-y-2 text-sm text-gray-600">
                    <div className="flex items-center gap-2">
                      <Building2 className="h-4 w-4" />
                      <span>Property ID: {listing.id.slice(0, 8).toUpperCase()}</span>
                    </div>
                  </div>
                </CardContent>
              </Card>
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
            "@type": listing.listingType === "Sale" ? "RealEstateListing" : "Apartment",
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

