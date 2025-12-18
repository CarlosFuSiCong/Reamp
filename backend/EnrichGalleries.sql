SET QUOTED_IDENTIFIER ON;
GO

-- Add multiple images to each listing for a richer gallery

-- Get all listings
DECLARE @AllListings TABLE (ListingId UNIQUEIDENTIFIER, Title NVARCHAR(160), CurrentImageCount INT, RowNum INT);
INSERT INTO @AllListings (ListingId, Title, CurrentImageCount, RowNum)
SELECT 
    l.Id, 
    l.Title, 
    COUNT(lmr.Id) as CurrentImageCount,
    ROW_NUMBER() OVER (ORDER BY l.CreatedAtUtc DESC)
FROM Listings l
LEFT JOIN ListingMediaRefs lmr ON l.Id = lmr.ListingId
WHERE l.Status = 1
GROUP BY l.Id, l.Title, l.CreatedAtUtc;

-- Get available media not yet used (we have 13 total images, 9 are used as covers)
DECLARE @UnusedMedia TABLE (MediaId UNIQUEIDENTIFIER, FileName NVARCHAR(255), RowNum INT);
INSERT INTO @UnusedMedia (MediaId, FileName, RowNum)
SELECT 
    ma.Id,
    ma.OriginalFileName,
    ROW_NUMBER() OVER (ORDER BY ma.CreatedAtUtc)
FROM MediaAssets ma
WHERE ma.OwnerStudioId = '968BD934-62F6-4452-B90A-0F704E8B3830'
  AND ma.ResourceType = 1
  AND NOT EXISTS (
      SELECT 1 FROM ListingMediaRefs lmr WHERE lmr.MediaAssetId = ma.Id
  );

PRINT 'Unused media available:';
SELECT COUNT(*) as UnusedCount FROM @UnusedMedia;

-- Add 2-3 more images to first 4 listings (to create nice galleries)
-- Listing 1: Add 2 images
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT TOP 2
    NEWID(),
    (SELECT TOP 1 ListingId FROM @AllListings WHERE RowNum = 1),
    MediaId,
    0,  -- General
    0,  -- Not cover
    1,  -- Visible
    RowNum  -- Sort order
FROM @UnusedMedia
WHERE RowNum <= 2;

-- Listing 2: Add 2 images
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT TOP 2
    NEWID(),
    (SELECT TOP 1 ListingId FROM @AllListings WHERE RowNum = 2),
    MediaId,
    0,
    0,
    1,
    RowNum + 10
FROM @UnusedMedia
WHERE RowNum BETWEEN 3 AND 4;

PRINT 'Added additional images to listings';

-- Final verification
SELECT 
    l.Title,
    COUNT(lmr.Id) as TotalImages,
    SUM(CASE WHEN lmr.IsCover = 1 THEN 1 ELSE 0 END) as CoverImages,
    COUNT(DISTINCT la.Id) as Agents
FROM Listings l
LEFT JOIN ListingMediaRefs lmr ON l.Id = lmr.ListingId
LEFT JOIN ListingAgentSnapshots la ON l.Id = la.ListingId
WHERE l.Status = 1
GROUP BY l.Title
ORDER BY TotalImages DESC, l.Title;

PRINT 'Gallery enrichment complete!';
