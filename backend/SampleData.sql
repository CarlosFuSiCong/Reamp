-- Sample Data for Reamp Platform Demo
-- This script creates sample listings with placeholder data for demonstration

-- Note: This assumes you already have:
-- 1. A registered Agency with an Agent user
-- 2. A registered Studio with Staff
-- Replace the IDs below with actual IDs from your database

-- ============================================
-- SAMPLE LISTINGS DATA
-- ============================================

DECLARE @AgencyId1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Agencies ORDER BY CreatedAtUtc);
DECLARE @AgentUserId1 UNIQUEIDENTIFIER = (SELECT TOP 1 UserProfileId FROM Agents WHERE AgencyId = @AgencyId1);

IF @AgencyId1 IS NOT NULL AND @AgentUserId1 IS NOT NULL
BEGIN
    -- Sample Listing 1: Luxury Villa
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Modern Luxury Villa with Ocean View',
        'Stunning modern villa featuring breathtaking ocean views, open-plan living spaces, and high-end finishes throughout. This exquisite property offers 5 spacious bedrooms, each with en-suite bathrooms, a gourmet kitchen with premium appliances, and expansive outdoor terraces perfect for entertaining. The infinity pool seamlessly blends with the horizon, creating a resort-like atmosphere. Located in an exclusive gated community with 24/7 security.',
        2500000, 'USD', 1, 1, 3,
        5, 5, 3, 450.0, 800.0,
        '123 Ocean Drive', 'Beachfront Estate', 'Miami', 'Florida', '33139', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 2: Downtown Luxury Apartment
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Downtown Luxury Apartment with City Views',
        'Experience urban luxury in this sophisticated downtown apartment. Floor-to-ceiling windows offer panoramic city views, while the open-concept design maximizes space and natural light. Features include hardwood floors, designer kitchen with marble countertops, spa-like master bathroom, and access to building amenities including gym, pool, and concierge service.',
        850000, 'USD', 1, 1, 4,
        3, 2, 2, 180.0, NULL,
        '456 Downtown Boulevard', 'Apt 2501', 'San Francisco', 'California', '94102', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 3: Charming Family Townhouse
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Charming Family Townhouse in Quiet Neighborhood',
        'Perfect family home in a peaceful, tree-lined neighborhood. This beautifully maintained townhouse features 4 bedrooms, 3 bathrooms, and a renovated kitchen with modern appliances. The private backyard is ideal for outdoor activities and includes a deck for dining. Walking distance to excellent schools, parks, and shopping centers. Move-in ready!',
        650000, 'USD', 1, 1, 2,
        4, 3, 2, 220.0, 150.0,
        '789 Maple Street', NULL, 'Austin', 'Texas', '78701', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 4: Spacious 2BR Apartment for Rent
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Spacious 2BR Apartment Available for Rent',
        'Bright and spacious 2-bedroom apartment in prime location. Features include updated kitchen, in-unit washer/dryer, hardwood floors throughout, and plenty of storage. Building offers elevator access, bike storage, and on-site management. Close to public transportation, restaurants, and entertainment. Pet-friendly building. Available immediately.',
        2800, 'USD', 1, 2, 4,
        2, 1, 1, 95.0, NULL,
        '321 Park Avenue', 'Unit 8B', 'Seattle', 'Washington', '98101', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 5: Cozy Suburban House
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Cozy Suburban House with Large Backyard',
        'Charming single-family home perfect for first-time buyers or growing families. Features 3 bedrooms, 2 bathrooms, updated kitchen, and a spacious living room with fireplace. The large fenced backyard is great for kids and pets. Attached 2-car garage with extra storage. Quiet neighborhood with excellent schools and community amenities.',
        425000, 'USD', 1, 1, 1,
        3, 2, 2, 170.0, 400.0,
        '567 Oak Lane', NULL, 'Portland', 'Oregon', '97201', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 6: Modern Studio Loft
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Modern Studio Loft in Arts District',
        'Stylish studio loft in the heart of the vibrant Arts District. High ceilings, exposed brick, large windows, and polished concrete floors create an industrial-chic atmosphere. Updated bathroom and kitchenette. Perfect for creative professionals or as an investment property. Walk to galleries, cafes, and entertainment venues. Bike storage available.',
        1800, 'USD', 1, 2, 4,
        0, 1, 0, 55.0, NULL,
        '890 Arts Boulevard', 'Loft 401', 'Denver', 'Colorado', '80202', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 7: Waterfront Estate
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Waterfront Estate with Private Dock',
        'Extraordinary waterfront estate offering unparalleled luxury and privacy. This magnificent property features 6 bedrooms, 7 bathrooms, gourmet chef''s kitchen, home theater, wine cellar, and gym. Expansive outdoor living spaces include infinity pool, spa, outdoor kitchen, and private dock accommodating large vessels. Smart home technology throughout. Gated entrance with guest house.',
        4800000, 'USD', 1, 1, 3,
        6, 7, 4, 650.0, 2000.0,
        '1001 Waterfront Drive', 'Private Island Estate', 'Naples', 'Florida', '34102', 'US',
        GETUTCDATE(), GETUTCDATE()
    );
    
    -- Sample Listing 8: Renovated Victorian Townhouse
    INSERT INTO Listings (
        Id, OwnerAgencyId, AgentUserId, Title, Description,
        Price, Currency, Status, ListingType, PropertyType,
        Bedrooms, Bathrooms, ParkingSpaces, FloorAreaSqm, LandAreaSqm,
        Address_Line1, Address_Line2, Address_City, Address_State, Address_Postcode, Address_Country,
        CreatedAtUtc, UpdatedAtUtc
    ) VALUES (
        NEWID(), @AgencyId1, @AgentUserId1,
        'Renovated Victorian Townhouse',
        'Beautifully restored Victorian townhouse combining historic charm with modern amenities. Original architectural details including crown molding, hardwood floors, and decorative fireplaces have been meticulously preserved. Updated systems, chef''s kitchen, and luxurious bathrooms provide contemporary comfort. Private garden and rooftop deck. Prime location near shops and restaurants.',
        1250000, 'USD', 1, 1, 2,
        4, 3, 1, 280.0, 80.0,
        '234 Heritage Street', NULL, 'Boston', 'Massachusetts', '02108', 'US',
        GETUTCDATE(), GETUTCDATE()
    );

    PRINT 'Sample listings created successfully!';
END
ELSE
BEGIN
    PRINT 'Please create an Agency and Agent first before running this script.';
END
