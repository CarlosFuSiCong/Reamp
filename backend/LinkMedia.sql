SET QUOTED_IDENTIFIER ON;
GO

-- Link uploaded media to listings

-- Get first 5 listings and media
DECLARE @Listings TABLE (ListingId UNIQUEIDENTIFIER, RowNum INT);
DECLARE @Media TABLE (MediaId UNIQUEIDENTIFIER, RowNum INT);

INSERT INTO @Listings (ListingId, RowNum)
SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAtUtc DESC)
FROM Listings 
WHERE Status = 1
ORDER BY CreatedAtUtc DESC;

INSERT INTO @Media (MediaId, RowNum)
SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAtUtc DESC)
FROM MediaAssets 
WHERE OwnerStudioId = '968BD934-62F6-4452-B90A-0F704E8B3830'
  AND ResourceType = 1  -- Image
ORDER BY CreatedAtUtc DESC;

-- Create ListingMediaRefs
INSERT INTO ListingMediaRefs (
    Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder
)
SELECT 
    NEWID(),
    l.ListingId,
    m.MediaId,
    0,  -- Role: General
    CASE WHEN m.RowNum = 1 THEN 1 ELSE 0 END,  -- First image is cover
    1,  -- IsVisible
    m.RowNum - 1  -- SortOrder (0-based)
FROM @Listings l
INNER JOIN @Media m ON l.RowNum = m.RowNum
WHERE l.RowNum <= 5;

SELECT @@ROWCOUNT as LinksCreated;

-- Verify
SELECT 
    l.Title,
    ma.OriginalFileName,
    lmr.IsCover,
    lmr.SortOrder
FROM ListingMediaRefs lmr
INNER JOIN Listings l ON lmr.ListingId = l.Id
INNER JOIN MediaAssets ma ON lmr.MediaAssetId = ma.Id
WHERE l.Status = 1
ORDER BY l.CreatedAtUtc DESC, lmr.SortOrder;

PRINT 'Successfully linked media to listings!';
