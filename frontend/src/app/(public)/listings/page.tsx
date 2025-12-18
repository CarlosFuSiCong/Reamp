"use client";

import { useSearchParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { listingsApi } from "@/lib/api";
import { ListingCard, ListingsSearch } from "@/components/public";
import { LoadingState, ErrorState } from "@/components/shared";
import { Footer } from "@/components/layout";
import { Navbar } from "@/components/layout";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight } from "lucide-react";
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
    <div className="min-h-screen flex flex-col">
      <Navbar />

      <main className="flex-1">
        {/* Hero Section */}
        <section className="bg-gradient-to-br from-blue-600 to-blue-800 text-white py-16">
          <div className="container mx-auto px-4 sm:px-6 lg:px-8">
            <div className="max-w-3xl mx-auto text-center mb-8">
              <h1 className="text-4xl sm:text-5xl font-bold mb-4">
                Find Your Dream Property
              </h1>
              <p className="text-xl text-blue-100">
                Browse our curated collection of professionally photographed properties
              </p>
            </div>

            {/* Search Bar */}
            <div className="max-w-4xl mx-auto">
              <ListingsSearch />
            </div>
          </div>
        </section>

        {/* Results Section */}
        <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-12">
          {isLoading ? (
            <LoadingState message="Loading properties..." />
          ) : error ? (
            <ErrorState
              title="Failed to load listings"
              message="Unable to fetch property listings. Please try again later."
            />
          ) : data?.items.length === 0 ? (
            <div className="text-center py-12">
              <div className="max-w-md mx-auto">
                <div className="w-24 h-24 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <svg
                    className="w-12 h-12 text-gray-400"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
                    />
                  </svg>
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-2">
                  No properties found
                </h3>
                <p className="text-gray-600">
                  Try adjusting your search filters or check back later for new listings.
                </p>
              </div>
            </div>
          ) : (
            <>
              {/* Results Count */}
              <div className="mb-6">
                <p className="text-gray-600">
                  Showing <span className="font-semibold">{data.items.length}</span> of{" "}
                  <span className="font-semibold">{data.totalCount}</span> properties
                </p>
              </div>

              {/* Listings Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-8">
                {data.items.map((listing) => (
                  <ListingCard key={listing.id} listing={listing} />
                ))}
              </div>

              {/* Pagination */}
              {data.totalPages > 1 && (
                <div className="flex justify-center items-center gap-2">
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() => handlePageChange(page - 1)}
                    disabled={page === 1}
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

