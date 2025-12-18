SET QUOTED_IDENTIFIER ON;
GO

-- Add floor plans to featured listings

-- Get the uploaded floor plan media IDs
DECLARE @FloorPlans TABLE (MediaId UNIQUEIDENTIFIER, RowNum INT);
INSERT INTO @FloorPlans
SELECT TOP 4
    ma.Id,
    ROW_NUMBER() OVER (ORDER BY ma.CreatedAtUtc DESC)
FROM MediaAssets ma
WHERE ma.OwnerStudioId = '968BD934-62F6-4452-B90A-0F704E8B3830'
  AND ma.ResourceType = 1
  AND NOT EXISTS (SELECT 1 FROM ListingMediaRefs lmr WHERE lmr.MediaAssetId = ma.Id)
ORDER BY ma.CreatedAtUtc DESC;

PRINT 'Available floor plans:';
SELECT * FROM @FloorPlans;

-- Get featured listings with most images
DECLARE @FeaturedListings TABLE (ListingId UNIQUEIDENTIFIER, Title NVARCHAR(160), RowNum INT);
INSERT INTO @FeaturedListings
SELECT TOP 4
    l.Id,
    l.Title,
    ROW_NUMBER() OVER (ORDER BY l.Title)
FROM Listings l
WHERE l.Status = 1
  AND l.Id IN (
    '08CAF562-FB4C-4AD5-8860-B29DF411F2FC', -- Charming Family Townhouse
    'FA76777F-1FA1-4008-A053-A88F327B3303', -- Downtown Luxury Apartment
    'B8C56668-4DF0-40E8-AEC7-3E6AAA7BB76D', -- Renovated Victorian Townhouse
    'FEC39A17-6816-42DD-BDF3-39A7096413E7'  -- House
  );

PRINT '';
PRINT 'Featured listings:';
SELECT * FROM @FeaturedListings;

-- Add one floor plan to each featured listing with Role = FloorPlan (3)
-- Listing 1: Charming Family Townhouse
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT 
    NEWID(),
    (SELECT ListingId FROM @FeaturedListings WHERE RowNum = 1),
    (SELECT MediaId FROM @FloorPlans WHERE RowNum = 1),
    3, -- FloorPlan role
    0, 1, 100; -- High sort order to show after photos

-- Listing 2: Downtown Luxury Apartment
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT 
    NEWID(),
    (SELECT ListingId FROM @FeaturedListings WHERE RowNum = 2),
    (SELECT MediaId FROM @FloorPlans WHERE RowNum = 2),
    3, 0, 1, 101;

-- Listing 3: House
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT 
    NEWID(),
    (SELECT ListingId FROM @FeaturedListings WHERE RowNum = 3),
    (SELECT MediaId FROM @FloorPlans WHERE RowNum = 3),
    3, 0, 1, 102;

-- Listing 4: Renovated Victorian Townhouse
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT 
    NEWID(),
    (SELECT ListingId FROM @FeaturedListings WHERE RowNum = 4),
    (SELECT MediaId FROM @FloorPlans WHERE RowNum = 4),
    3, 0, 1, 103;

PRINT '';
PRINT 'Floor plans added successfully!';
PRINT '';

-- Final result - Show media by role
SELECT 
    l.Title,
    COUNT(CASE WHEN lmr.Role = 0 THEN 1 END) as Photos,
    COUNT(CASE WHEN lmr.Role = 3 THEN 1 END) as FloorPlans,
    COUNT(lmr.Id) as TotalMedia
FROM Listings l
LEFT JOIN ListingMediaRefs lmr ON l.Id = lmr.ListingId
WHERE l.Status = 1
GROUP BY l.Title
ORDER BY FloorPlans DESC, Photos DESC, l.Title;

PRINT 'Floor plans integration complete!';
