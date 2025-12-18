SET QUOTED_IDENTIFIER ON;
GO

-- Add more images to remaining listings and add agents to all listings

-- Get Agent info
DECLARE @AgencyId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Agencies ORDER BY CreatedAtUtc);
DECLARE @AgentEmail NVARCHAR(256) = (SELECT TOP 1 u.Email FROM Agents a INNER JOIN AspNetUsers u ON a.UserProfileId = u.Id WHERE a.AgencyId = @AgencyId);

-- Get listings without images (4 listings)
DECLARE @ListingsNoImages TABLE (ListingId UNIQUEIDENTIFIER, Title NVARCHAR(160), RowNum INT);
INSERT INTO @ListingsNoImages (ListingId, Title, RowNum)
SELECT l.Id, l.Title, ROW_NUMBER() OVER (ORDER BY l.CreatedAtUtc DESC)
FROM Listings l
WHERE l.Status = 1
  AND NOT EXISTS (SELECT 1 FROM ListingMediaRefs lmr WHERE lmr.ListingId = l.Id);

-- Get new uploaded media (last 4 images)
DECLARE @NewMedia TABLE (MediaId UNIQUEIDENTIFIER, FileName NVARCHAR(255), RowNum INT);
INSERT INTO @NewMedia (MediaId, FileName, RowNum)
SELECT TOP 4 Id, OriginalFileName, ROW_NUMBER() OVER (ORDER BY CreatedAtUtc DESC)
FROM MediaAssets 
WHERE OwnerStudioId = '968BD934-62F6-4452-B90A-0F704E8B3830'
  AND ResourceType = 1
ORDER BY CreatedAtUtc DESC;

PRINT 'Found listings without images:';
SELECT * FROM @ListingsNoImages;

PRINT 'Found new media:';
SELECT * FROM @NewMedia;

-- Link new media to listings
INSERT INTO ListingMediaRefs (Id, ListingId, MediaAssetId, Role, IsCover, IsVisible, SortOrder)
SELECT 
    NEWID(),
    l.ListingId,
    m.MediaId,
    0,  -- Role: General
    1,  -- First image is cover
    1,  -- IsVisible
    0   -- SortOrder
FROM @ListingsNoImages l
INNER JOIN @NewMedia m ON l.RowNum = m.RowNum;

PRINT 'Linked images to listings without images';

-- Add agent snapshots to ALL listings that don't have agents
INSERT INTO ListingAgentSnapshots (
    Id, ListingId,
    FirstName, LastName, Email, PhoneNumber,
    IsPrimary, SortOrder
)
SELECT 
    NEWID(),
    l.Id,
    'John',  -- FirstName
    'Smith', -- LastName
    ISNULL(@AgentEmail, 'agent1@reamp.com'),
    '+1 (555) 123-4567',
    1,  -- IsPrimary
    0   -- SortOrder
FROM Listings l
WHERE l.Status = 1
  AND NOT EXISTS (SELECT 1 FROM ListingAgentSnapshots la WHERE la.ListingId = l.Id);

PRINT 'Added agent snapshots to all listings';

-- Verify results
SELECT 
    l.Title,
    COUNT(DISTINCT lmr.Id) as ImageCount,
    COUNT(DISTINCT la.Id) as AgentCount
FROM Listings l
LEFT JOIN ListingMediaRefs lmr ON l.Id = lmr.ListingId
LEFT JOIN ListingAgentSnapshots la ON l.Id = la.ListingId
WHERE l.Status = 1
GROUP BY l.Title
ORDER BY ImageCount DESC;

PRINT 'All listings updated successfully!';
