# Avatar Upload 404 Fix

## Problem Summary
Users were getting 404 errors when trying to view their uploaded avatar images from Cloudinary.

Example error:
```
GET https://res.cloudinary.com/dccearggm/image/upload/w_200,h_200,c_fill,g_face,q_auto,f_auto/reamp-dev/rilvztaedzsku1ptsqfg.jpg
Status: 404 Not Found
```

## Root Cause

The backend was storing `PublicUrl` with transformation parameters baked into the URL (from `EnableAutoOptimization` setting):

```csharp
// Before: SecureUrl from Cloudinary included transformations
uploadParams.Transformation = new Transformation()
    .Quality("auto")
    .FetchFormat("auto");

// This resulted in SecureUrl like:
// https://res.cloudinary.com/.../upload/q_auto,f_auto/reamp-dev/xyz.jpg
```

When the frontend applied additional transformations for avatars, the URL parsing/rebuilding could lead to issues or the image might not have been uploaded successfully in the first place.

## Solutions Implemented

### 1. Store Clean Base URLs (Backend)

**File**: `backend/src/Reamp/Reamp.Infrastructure/Services/Media/CloudinaryService.cs`

Changed to store base URLs without any transformations:

```csharp
// Build base URL without transformations for storage
var baseUrl = $"https://res.cloudinary.com/{_settings.CloudName}/image/upload/{uploadResult.PublicId}.{uploadResult.Format}";

return new CloudinaryUploadResult
{
    PublicId = uploadResult.PublicId,
    SecureUrl = baseUrl, // Clean base URL
    // ...
};
```

**Benefits**:
- Clean URLs in database
- Frontend can reliably apply transformations
- No conflicts between storage transformations and display transformations

### 2. Enhanced Logging (Backend)

**Files**:
- `backend/src/Reamp/Reamp.Api/Controllers/Media/MediaController.cs`
- `backend/src/Reamp/Reamp.Application/Media/Services/MediaAssetAppService.cs`
- `backend/src/Reamp/Reamp.Infrastructure/Services/Media/CloudinaryService.cs`

Added detailed logging for:
- Upload initiation
- Cloudinary response (PublicId, SecureUrl, Format)
- Final stored asset details

Example logs:
```
Uploading avatar for user {UserId}
Cloudinary upload successful: PublicId={PublicId}, SecureUrl={SecureUrl}, Format={Format}
Avatar uploaded successfully: AssetId={AssetId}, PublicUrl={PublicUrl}, ProviderAssetId={ProviderAssetId}
```

### 3. Better Error Handling (Frontend)

**File**: `frontend/src/components/profile/avatar-upload.tsx`

Added:
- Console logging for debugging
- Image load error handler
- Fallback to Avatar with initials

```tsx
<AvatarImage 
  src={applyCloudinaryPreset(avatarUrl, "avatar")}
  onError={(e) => {
    console.error("[AvatarUpload] Image failed to load:", {
      src: e.currentTarget.src,
      avatarUrl,
      preview,
    });
    e.currentTarget.src = ""; // Triggers fallback
  }}
/>
<AvatarFallback>{displayName?.charAt(0).toUpperCase() || "U"}</AvatarFallback>
```

### 4. Debug Logging (Frontend)

**File**: `frontend/src/lib/utils/cloudinary.ts`

Added development-mode logging for URL transformations:

```typescript
if (process.env.NODE_ENV === 'development') {
  console.debug('[Cloudinary] Transform URL:', {
    original: url,
    parsed: { cloudName, publicId, extension },
    transformString,
    result: transformedUrl
  });
}
```

## Migration Note

**Existing images in the database** with transformation parameters in their `PublicUrl` will continue to work because:
1. The frontend's `parseCloudinaryUrl()` function strips out transformation parameters
2. New transformations are applied on top of the clean public_id

**However**, if you want to clean up existing data:

```sql
-- For images uploaded with transformations, strip them to base URLs
-- This is OPTIONAL but recommended for consistency

UPDATE MediaAssets
SET PublicUrl = CONCAT(
    'https://res.cloudinary.com/dccearggm/image/upload/',
    ProviderAssetId,
    '.',
    -- Extract format from ContentType
    CASE 
        WHEN ContentType = 'image/jpeg' THEN 'jpg'
        WHEN ContentType = 'image/png' THEN 'png'
        WHEN ContentType = 'image/webp' THEN 'webp'
        WHEN ContentType = 'image/gif' THEN 'gif'
        ELSE 'jpg'
    END
)
WHERE Provider = 0  -- Cloudinary
AND PublicUrl LIKE '%q_auto%'  -- Has transformations
```

## Testing Checklist

### Upload New Avatar
1. ✅ Navigate to Profile page
2. ✅ Select an image file
3. ✅ Click Upload
4. ✅ Verify image displays immediately
5. ✅ Check browser console for logs
6. ✅ Check backend logs for upload details
7. ✅ Refresh page and verify avatar persists

### Check Cloudinary
1. ✅ Login to Cloudinary console
2. ✅ Navigate to Media Library
3. ✅ Look for `reamp-dev` folder (dev) or `reamp` folder (prod)
4. ✅ Verify new uploads appear
5. ✅ Check public_id matches database

### Verify Database
```sql
SELECT 
    Id,
    ProviderAssetId,
    PublicUrl,
    ContentType,
    ProcessStatus,
    CreatedAtUtc
FROM MediaAssets
WHERE OwnerStudioId = '00000000-0000-0000-0000-000000000000'  -- User-owned
ORDER BY CreatedAtUtc DESC
```

## Future Improvements

### 1. Proactive Health Check
Add endpoint to verify Cloudinary images exist:

```csharp
[HttpPost("verify/{id:guid}")]
public async Task<IActionResult> VerifyMediaExists(Guid id)
{
    // HEAD request to Cloudinary URL
    // Update ProcessStatus if image is missing
}
```

### 2. Automatic Variant Generation
Generate avatar-specific variants during upload:

```csharp
if (isImage && dto.OwnerStudioId == Guid.Empty) // User avatar
{
    var avatarUrl = _cloudinaryService.GenerateTransformationUrl(
        uploadResult.PublicId, 200, 200, MediaResourceType.Image);
    
    mediaAsset.AddOrReplaceVariant(
        name: "avatar",
        url: avatarUrl,
        width: 200,
        height: 200);
}
```

### 3. CDN Fallback
If Cloudinary image fails, fall back to different source:

```tsx
const [imgSrc, setImgSrc] = useState(cloudinaryUrl);

<AvatarImage 
  src={imgSrc}
  onError={() => {
    if (imgSrc.includes('cloudinary')) {
      setImgSrc(backupCdnUrl);
    }
  }}
/>
```

## Related Files

### Backend
- `backend/src/Reamp/Reamp.Infrastructure/Services/Media/CloudinaryService.cs`
- `backend/src/Reamp/Reamp.Application/Media/Services/MediaAssetAppService.cs`
- `backend/src/Reamp/Reamp.Api/Controllers/Media/MediaController.cs`

### Frontend
- `frontend/src/components/profile/avatar-upload.tsx`
- `frontend/src/lib/utils/cloudinary.ts`
- `frontend/src/lib/api/media.ts`

### Configuration
- `backend/src/Reamp/Reamp.Api/appsettings.Development.json`
  - `Cloudinary.Folder`: `"reamp-dev"`
  - `MediaUpload.EnableAutoOptimization`: `true`
