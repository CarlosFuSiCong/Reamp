-- Fix admin@reamp.com role to Admin (4)
-- This script updates the UserProfile table to set the correct role for the admin account

USE ReampDb;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Update the role for admin@reamp.com
UPDATE UserProfiles
SET Role = 4  -- Admin role
WHERE ApplicationUserId = (
    SELECT Id 
    FROM AspNetUsers 
    WHERE Email = 'admin@reamp.com'
);

-- Verify the update
SELECT 
    u.Email,
    up.FirstName,
    up.LastName,
    up.Role,
    CASE up.Role
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'User'
        WHEN 2 THEN 'Client'
        WHEN 3 THEN 'Staff'
        WHEN 4 THEN 'Admin'
        ELSE 'Unknown'
    END AS RoleName
FROM AspNetUsers u
INNER JOIN UserProfiles up ON u.Id = up.ApplicationUserId
WHERE u.Email = 'admin@reamp.com';

PRINT 'Admin role updated successfully!';
GO
