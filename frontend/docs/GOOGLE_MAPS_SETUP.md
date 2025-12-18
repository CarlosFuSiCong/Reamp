# Google Maps Integration

This project integrates Google Maps for address autocomplete and property location display.

## Setup

### 1. Get Google Maps API Key

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the following APIs:
   - **Maps JavaScript API**
   - **Places API**
   - **Geocoding API**
4. Go to "Credentials" and create an API key
5. (Recommended) Restrict the API key:
   - **Application restrictions**: HTTP referrers
   - **Website restrictions**: Add your domain (e.g., `localhost:3000/*`, `yourdomain.com/*`)
   - **API restrictions**: Restrict to the APIs listed above

### 2. Configure Environment Variables

Add your Google Maps API key to your `.env.local` file:

```bash
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=your_google_maps_api_key_here
```

**Note**: Copy `env.example` to `.env.local` if you haven't already:

```bash
cp env.example .env.local
```

### 3. (Optional) Configure Map ID

For advanced map styling, you can create a custom Map ID:

1. In Google Cloud Console, go to "Map Styles"
2. Create a new map style
3. Copy the Map ID
4. Add it to your `.env.local`:

```bash
NEXT_PUBLIC_GOOGLE_MAPS_MAP_ID=your_map_id_here
```

## Features

### Address Autocomplete

The address autocomplete component provides:
- Real-time address suggestions as you type
- Automatic parsing of address components (street, city, state, postcode, country)
- Geolocation coordinates (latitude/longitude) extraction
- Fallback to manual input if API key is not configured
- Support for multiple countries (AU, US, GB, NZ, CA)

**Location**: `src/components/maps/address-autocomplete.tsx`

### Property Map Display

The map component displays:
- Interactive Google Map with the property location
- Custom marker at the property coordinates
- Graceful fallback UI when coordinates are not available
- Responsive design with configurable height

**Location**: `src/components/maps/property-map.tsx`

## Usage

### In Listing Forms

The address fields automatically use Google Maps autocomplete:

```tsx
import { ListingAddressFields } from "@/components/listings";

<ListingAddressFields form={form} />
```

Users can:
- Use address autocomplete (default)
- Switch to manual entry
- Edit individual address fields after autocomplete

### In Property Details

The map automatically displays if coordinates are available:

```tsx
import { PropertyMap } from "@/components/maps";

<PropertyMap
  latitude={listing.latitude}
  longitude={listing.longitude}
  address={fullAddress}
  title="Property Location"
  zoom={15}
  height="400px"
/>
```

## Fallback Behavior

The integration is designed to work gracefully without a configured API key:

- **Address Autocomplete**: Falls back to standard text input
- **Map Display**: Shows a placeholder with address text
- **No Errors**: Application continues to function normally

## Cost Considerations

Google Maps APIs have different pricing tiers:

- **Places Autocomplete**: $2.83 per 1,000 requests (after free tier)
- **Geocoding**: $5.00 per 1,000 requests (after free tier)
- **Maps JavaScript API**: $7.00 per 1,000 loads (after free tier)

**Free Tier**: $200 USD monthly credit (equivalent to ~28,000 map loads or ~70,000 autocomplete requests)

For a typical real estate platform with moderate traffic, the free tier should be sufficient.

## Troubleshooting

### API Key Not Working

1. Verify the API key is correctly set in `.env.local`
2. Ensure you've enabled all required APIs in Google Cloud Console
3. Check API key restrictions aren't blocking your domain
4. Restart the development server after changing `.env.local`

### Address Autocomplete Not Appearing

1. Open browser DevTools console and check for errors
2. Verify "Places API" is enabled in Google Cloud Console
3. Check network tab for API request failures
4. Ensure country restrictions match your target region

### Map Not Displaying

1. Check if coordinates (latitude/longitude) are available
2. Verify "Maps JavaScript API" is enabled
3. Check for console errors related to map initialization
4. Ensure API key has permission for Maps JavaScript API

## Security Best Practices

1. **Never commit** your actual API key to version control
2. Use **API key restrictions** to limit usage to your domains
3. Monitor **API usage** in Google Cloud Console
4. Set up **billing alerts** to avoid unexpected charges
5. Use **separate keys** for development and production

## References

- [Google Maps JavaScript API Documentation](https://developers.google.com/maps/documentation/javascript)
- [Places API Documentation](https://developers.google.com/maps/documentation/places/web-service)
- [Geocoding API Documentation](https://developers.google.com/maps/documentation/geocoding)
- [Google Maps Pricing](https://mapsplatform.google.com/pricing/)
