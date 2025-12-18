SET QUOTED_IDENTIFIER ON;
GO

-- Distribute new images to all listings to create rich galleries

-- Get all listings with their current image count
DECLARE @Listings TABLE (ListingId UNIQUEIDENTIFIER, Title NVARCHAR(160), CurrentImages INT, RowNum INT);
INSERT INTO @Listings
SELECT 
    l.Id,
    l.Title,
    COUNT(lmr.Id) as CurrentImages,
    ROW_NUMBER() OVER (ORDER BY COUNT(lmr.Id), l.Title)
FROM Listings l
LEFT JOIN ListingMediaRefs lmr ON l.Id = lmr.ListingId
WHERE l.Status = 1
GROUP BY l.Id, l.Title;

-- Get new unused media
DECLARE @NewMedia TABLE (MediaId UNIQUEIDENTIFIER, RowNum INT);
INSERT INTO @NewMedia
SELECT TOP 20
    ma.Id,
    ROW_NUMBER() OVER (ORDER BY ma.CreatedAtUtc DESC)
FROM MediaAssets ma
WHERE ma.OwnerStudioId = '968BD934-62F6-4452-B90A-0F704E8B3830'
  AND ma.ResourceType = 1
  AND NOT EXISTS (SELECT 1 FROM ListingMediaRefs lmr WHERE lmr.MediaAssetId = ma.Id)
ORDER BY ma.CreatedAtUtc DESC;

PRINT 'Current state:';
SELECT * FROM @Listings ORDER BY RowNum;

PRINT '';
PRINT 'Available new media:';
SELECT COUNT(*) as Count FROM @NewMedia;

-- Add 2-3 images to each listing
-- Listing 1: Add 3 images
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT TOP 3
    NEWID(),
    (SELECT ListingId FROM @Listings WHERE RowNum = 1),
    m.MediaId,
    0, 0, 1, m.RowNum
FROM @NewMedia m
WHERE m.RowNum BETWEEN 1 AND 3;

-- Listing 2: Add 3 images  
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT TOP 3
    NEWID(),
    (SELECT ListingId FROM @Listings WHERE RowNum = 2),
    m.MediaId,
    0, 0, 1, m.RowNum + 10
FROM @NewMedia m
WHERE m.RowNum BETWEEN 4 AND 6;

-- Listing 3: Add 2 images
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT TOP 2
    NEWID(),
    (SELECT ListingId FROM @Listings WHERE RowNum = 3),
    m.MediaId,
    0, 0, 1, m.RowNum + 20
FROM @NewMedia m
WHERE m.RowNum BETWEEN 7 AND 8;

-- Listing 4: Add 2 images
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT TOP 2
    NEWID(),
    (SELECT ListingId FROM @Listings WHERE RowNum = 4),
    m.MediaId,
    0, 0, 1, m.RowNum + 30
FROM @NewMedia m
WHERE m.RowNum = 9;

-- Listing 5: Add 1 image (we only have 9 new images)
IF EXISTS (SELECT 1 FROM @NewMedia WHERE RowNum = 9)
BEGIN
    INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
    SELECT 
        NEWID(),
        (SELECT ListingId FROM @Listings WHERE RowNum = 5),
        m.MediaId,
        0, 0, 1, 40
    FROM @NewMedia m
    WHERE m.RowNum = 9;
END

PRINT '';
PRINT 'Images added successfully!';
PRINT '';

-- Final result
SELECT 
    l.Title,
    COUNT(lmr.Id) as TotalImages,
    SUM(CASE WHEN lmr.IsCover = 1 THEN 1 ELSE 0 END) as Covers
FROM Listings l
LEFT JOIN ListingMediaRefs lmr ON l.Id = lmr.ListingId
WHERE l.Status = 1
GROUP BY l.Title
ORDER BY TotalImages DESC, l.Title;

PRINT 'Gallery enrichment complete!';
