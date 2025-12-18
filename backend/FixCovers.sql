SET QUOTED_IDENTIFIER ON;
GO

-- Fix cover images for listings with media but no cover

-- Update the first image of each listing to be the cover
WITH RankedMedia AS (
    SELECT 
        lmr.Id,
        lmr.ListingId,
        ROW_NUMBER() OVER (PARTITION BY lmr.ListingId ORDER BY lmr.SortOrder) as RowNum
    FROM ListingMediaRefs lmr
    INNER JOIN Listings l ON lmr.ListingId = l.Id
    WHERE l.Status = 1
)
UPDATE ListingMediaRefs
SET IsCover = 1
WHERE Id IN (
    SELECT Id FROM RankedMedia WHERE RowNum = 1
);

SELECT @@ROWCOUNT as UpdatedCovers;

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
ORDER BY l.Title, lmr.SortOrder;

PRINT 'Cover images updated successfully!';
