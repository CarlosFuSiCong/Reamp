"use client";

import { useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search, RotateCcw, Filter } from "lucide-react";
import { PropertyType, ListingType } from "@/types/enums";
import { getPropertyTypeLabel, getListingTypeLabel } from "@/lib/utils/enum-labels";
import { Separator } from "@/components/ui/separator";

export function ListingsSearch() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const [keyword, setKeyword] = useState(searchParams.get("keyword") || "");
  const [propertyType, setPropertyType] = useState(searchParams.get("property") || "");
  const [listingType, setListingType] = useState(searchParams.get("type") || "");
  const [minPrice, setMinPrice] = useState(searchParams.get("minPrice") || "");
  const [maxPrice, setMaxPrice] = useState(searchParams.get("maxPrice") || "");
  const [isAdvancedOpen, setIsAdvancedOpen] = useState(false);

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
    <div className="bg-white rounded-xl shadow-xl shadow-blue-900/5 p-6 border border-gray-100">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-4 items-end">
        {/* Keyword Search */}
        <div className="lg:col-span-10 space-y-2">
          <label className="text-sm font-medium text-gray-700 ml-1">Location or Keyword</label>
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
              placeholder="Search by city, address, or ID..."
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              className="w-full pl-10 h-11"
          />
          </div>
        </div>

        {/* Action Buttons */}
        <div className="lg:col-span-2 flex gap-2">
           <Button onClick={handleSearch} className="flex-1 h-11 bg-blue-600 hover:bg-blue-700 text-base shadow-lg shadow-blue-600/20">
            Search
          </Button>
           <Button 
            onClick={() => setIsAdvancedOpen(!isAdvancedOpen)} 
            variant="outline" 
            className={`h-11 w-11 p-0 flex-shrink-0 ${isAdvancedOpen ? 'bg-gray-100' : ''}`}
            title="Advanced Filters"
          >
            <Filter className="h-4 w-4 text-gray-600" />
          </Button>
        </div>
        </div>

      {/* Advanced Filters */}
      {isAdvancedOpen && (
        <>
          <Separator className="my-6" />
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 items-end animate-in fade-in slide-in-from-top-2 duration-200">
        {/* Property Type */}
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700 ml-1">Property Type</label>
        <Select value={propertyType} onValueChange={setPropertyType}>
                <SelectTrigger className="h-10">
                  <SelectValue placeholder="All Types" />
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
            </div>

        {/* Listing Type */}
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700 ml-1">Status</label>
        <Select value={listingType} onValueChange={setListingType}>
                <SelectTrigger className="h-10">
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
            </div>

        {/* Price Range */}
            <div className="md:col-span-2 space-y-2">
               <label className="text-sm font-medium text-gray-700 ml-1">Price Range</label>
               <div className="flex gap-3">
                  <div className="relative flex-1">
                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500">$</span>
          <Input
            type="number"
                      placeholder="Min"
            value={minPrice}
            onChange={(e) => setMinPrice(e.target.value)}
                      className="w-full pl-7 h-10"
          />
                  </div>
                  <span className="self-center text-gray-400">-</span>
                  <div className="relative flex-1">
                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500">$</span>
          <Input
            type="number"
                      placeholder="Max"
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
                      className="w-full pl-7 h-10"
          />
                  </div>
        </div>
      </div>

            <div className="md:col-span-4 flex justify-end mt-2">
               <Button onClick={handleReset} variant="ghost" className="text-gray-500 hover:text-red-600 hover:bg-red-50 gap-2 h-10">
                <RotateCcw className="h-4 w-4" />
                Reset Filters
        </Button>
      </div>
          </div>
        </>
      )}
    </div>
  );
}
