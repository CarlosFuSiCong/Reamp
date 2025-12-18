"use client";

import { useSearchParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "@/lib/api";
import { ListingCard, ListingsSearch } from "@/components/public";
import { LoadingState, ErrorState } from "@/components/shared";
import { Footer } from "@/components/layout";
import { Navbar } from "@/components/layout";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight, SearchX } from "lucide-react";
import { useState } from "react";

export default function ListingsPage() {
  const searchParams = useSearchParams();
  const [page, setPage] = useState(1);
  const pageSize = 12;

  // Build query params
  const queryParams = {
    status: "Active",
    keyword: searchParams.get("keyword") || undefined,
    property: searchParams.get("property") === "all" ? undefined : searchParams.get("property") || undefined,
    type: searchParams.get("type") === "all" ? undefined : searchParams.get("type") || undefined,
    page,
    pageSize,
  };

  const { data, isLoading, error } = useQuery({
    queryKey: ["public-listings", queryParams],
    queryFn: () => listingsApi.listPublic(queryParams),
  });

  const handlePageChange = (newPage: number) => {
    setPage(newPage);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  return (
    <div className="min-h-screen flex flex-col bg-gray-50/30">
      <Navbar />

      <main className="flex-1">
        {/* Hero Section */}
        <section className="relative bg-gradient-to-b from-blue-900 to-blue-800 text-white pt-20 pb-32 overflow-hidden">
          <div className="absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.05)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.05)_1px,transparent_1px)] bg-[size:40px_40px] opacity-20"></div>
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 relative z-10">
            <div className="max-w-3xl mx-auto text-center">
              <h1 className="text-4xl sm:text-5xl md:text-6xl font-bold mb-6 tracking-tight">
                Find Your Dream Property
              </h1>
              <p className="text-lg sm:text-xl text-blue-100 mb-8 max-w-2xl mx-auto font-light">
                Discover our curated collection of professionally photographed properties across the country.
              </p>
            </div>
          </div>
        </section>

        {/* Search Section - Floating overlap */}
        <section className="container mx-auto px-4 sm:px-6 lg:px-8 -mt-16 relative z-20 mb-12">
          <div className="max-w-5xl mx-auto">
            <ListingsSearch />
          </div>
        </section>

        {/* Results Section */}
        <section className="container mx-auto px-4 sm:px-6 lg:px-8 pb-20">
          {isLoading ? (
            <div className="py-20">
              <LoadingState message="Discovering properties..." />
            </div>
          ) : error ? (
            <div className="py-20">
              <ErrorState
                title="Failed to load listings"
                message="Unable to fetch property listings. Please try again later."
              />
            </div>
          ) : data?.items.length === 0 ? (
            <div className="text-center py-20 bg-white rounded-2xl shadow-sm border border-gray-100 max-w-2xl mx-auto">
              <div className="w-20 h-20 bg-gray-50 rounded-full flex items-center justify-center mx-auto mb-6">
                <SearchX className="w-10 h-10 text-gray-400" />
              </div>
              <h3 className="text-2xl font-bold text-gray-900 mb-3">
                No properties found
              </h3>
              <p className="text-gray-500 max-w-md mx-auto mb-8">
                We couldn't find any properties matching your search criteria. Try adjusting your filters or browsing all listings.
              </p>
              <Button onClick={() => window.location.href = '/listings'} variant="outline">
                Clear Filters
              </Button>
            </div>
          ) : (
            <>
              {/* Results Header */}
              <div className="flex flex-col sm:flex-row justify-between items-end sm:items-center mb-8 gap-4 border-b border-gray-100 pb-4">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">Available Properties</h2>
                  <p className="text-gray-500 mt-1">
                    Showing <span className="font-semibold text-gray-900">{data.items.length}</span> of{" "}
                    <span className="font-semibold text-gray-900">{data.totalCount}</span> results
                  </p>
                </div>
              </div>

              {/* Listings Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 lg:gap-8 mb-12">
                {data.items.map((listing) => (
                  <ListingCard key={listing.id} listing={listing} />
                ))}
              </div>

              {/* Pagination */}
              {data.totalPages > 1 && (
                <div className="flex justify-center items-center gap-2 mt-12">
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() => handlePageChange(page - 1)}
                    disabled={page === 1}
                    className="h-10 w-10"
                  >
                    <ChevronLeft className="h-4 w-4" />
                  </Button>

                  <div className="flex items-center gap-2">
                    {Array.from({ length: Math.min(5, data.totalPages) }, (_, i) => {
                      let pageNum: number;
                      if (data.totalPages <= 5) {
                        pageNum = i + 1;
                      } else if (page <= 3) {
                        pageNum = i + 1;
                      } else if (page >= data.totalPages - 2) {
                        pageNum = data.totalPages - 4 + i;
                      } else {
                        pageNum = page - 2 + i;
                      }

                      return (
                        <Button
                          key={pageNum}
                          variant={page === pageNum ? "default" : "outline"}
                          size="icon"
                          onClick={() => handlePageChange(pageNum)}
                          className={`h-10 w-10 ${page === pageNum ? 'bg-blue-600 hover:bg-blue-700' : ''}`}
                        >
                          {pageNum}
                        </Button>
                      );
                    })}
                  </div>

                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() => handlePageChange(page + 1)}
                    disabled={page === data.totalPages}
                    className="h-10 w-10"
                  >
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
              )}
            </>
          )}
        </section>
      </main>

      <Footer />
    </div>
  );
}
