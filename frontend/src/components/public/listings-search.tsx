"use client";

import { useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search } from "lucide-react";
import { PropertyType, ListingType } from "@/types/enums";
import { getPropertyTypeLabel, getListingTypeLabel } from "@/lib/utils/enum-labels";

export function ListingsSearch() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const [keyword, setKeyword] = useState(searchParams.get("keyword") || "");
  const [propertyType, setPropertyType] = useState(searchParams.get("property") || "");
  const [listingType, setListingType] = useState(searchParams.get("type") || "");
  const [minPrice, setMinPrice] = useState(searchParams.get("minPrice") || "");
  const [maxPrice, setMaxPrice] = useState(searchParams.get("maxPrice") || "");

  const handleSearch = () => {
    const params = new URLSearchParams();
    if (keyword) params.set("keyword", keyword);
    if (propertyType) params.set("property", propertyType);
    if (listingType) params.set("type", listingType);
    if (minPrice) params.set("minPrice", minPrice);
    if (maxPrice) params.set("maxPrice", maxPrice);

    router.push(`/listings?${params.toString()}`);
  };

  const handleReset = () => {
    setKeyword("");
    setPropertyType("");
    setListingType("");
    setMinPrice("");
    setMaxPrice("");
    router.push("/listings");
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {/* Keyword Search */}
        <div className="lg:col-span-3">
          <Input
            placeholder="Search by location, title, or keywords..."
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            className="w-full"
          />
        </div>

        {/* Property Type */}
        <Select value={propertyType} onValueChange={setPropertyType}>
          <SelectTrigger>
            <SelectValue placeholder="Property Type" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Types</SelectItem>
            {Object.values(PropertyType)
              .filter((v) => typeof v === "number")
              .map((type) => (
                <SelectItem key={type} value={type.toString()}>
                  {getPropertyTypeLabel(type as PropertyType)}
                </SelectItem>
              ))}
          </SelectContent>
        </Select>

        {/* Listing Type */}
        <Select value={listingType} onValueChange={setListingType}>
          <SelectTrigger>
            <SelectValue placeholder="Sale or Rent" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All</SelectItem>
            {Object.values(ListingType)
              .filter((v) => typeof v === "number")
              .map((type) => (
                <SelectItem key={type} value={type.toString()}>
                  {getListingTypeLabel(type as ListingType)}
                </SelectItem>
              ))}
          </SelectContent>
        </Select>

        {/* Price Range */}
        <div className="flex gap-2">
          <Input
            type="number"
            placeholder="Min Price"
            value={minPrice}
            onChange={(e) => setMinPrice(e.target.value)}
            className="w-full"
          />
          <Input
            type="number"
            placeholder="Max Price"
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
            className="w-full"
          />
        </div>
      </div>

      {/* Action Buttons */}
      <div className="flex gap-2 mt-4">
        <Button onClick={handleSearch} className="flex-1 gap-2">
          <Search className="h-4 w-4" />
          Search
        </Button>
        <Button onClick={handleReset} variant="outline">
          Reset
        </Button>
      </div>
    </div>
  );
}

