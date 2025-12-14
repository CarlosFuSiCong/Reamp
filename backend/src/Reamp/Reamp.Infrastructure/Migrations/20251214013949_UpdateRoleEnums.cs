using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update UserRole enum values
            // Old: None=0, User=1, Client=2, Staff=3, Admin=4
            // New: None=0, Client=1, Agent=2, Staff=3, Admin=4
            // Update User (1) -> Client (1) - No change needed for value
            // Update Client (2) -> Agent (2) - This needs to be changed
            
            // First, update any Client (old value 2) to a temporary value to avoid conflicts
            migrationBuilder.Sql(@"
                UPDATE UserProfiles 
                SET Role = 10 
                WHERE Role = 2;
            ");
            
            // Update User (old value 1) to Client (new value 1) - No change in value needed
            // Users with value 1 stay as 1 (but semantically changed from User to Client)
            
            // Update temporary Client (10) to Agent (new value 2)
            migrationBuilder.Sql(@"
                UPDATE UserProfiles 
                SET Role = 2 
                WHERE Role = 10;
            ");

            // Update AgencyRole enum values
            // Old: Member=0, Agent=1, Manager=2, Owner=3
            // New: Agent=1, Manager=2, Owner=3
            // Update any Member (0) to Agent (1)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Agents')
                BEGIN
                    UPDATE Agents 
                    SET Role = 1 
                    WHERE Role = 0;
                END
            ");

            // Update StudioRole enum values
            // Old: Member=0, Editor=1, Photographer=2, Manager=3, Owner=4
            // New: Staff=1, Manager=2, Owner=3
            // Update Member (0) -> Staff (1)
            // Update Editor (1) -> Staff (1) 
            // Update Photographer (2) -> Staff (1)
            // Update Manager (3) -> Manager (2)
            // Update Owner (4) -> Owner (3)
            
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff')
                BEGIN
                    UPDATE Staff 
                    SET Role = CASE 
                        WHEN Role = 0 THEN 1  -- Member -> Staff
                        WHEN Role = 1 THEN 1  -- Editor -> Staff
                        WHEN Role = 2 THEN 1  -- Photographer -> Staff
                        WHEN Role = 3 THEN 2  -- Manager -> Manager
                        WHEN Role = 4 THEN 3  -- Owner -> Owner
                        ELSE Role
                    END;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert UserRole enum values
            // New: None=0, Client=1, Agent=2, Staff=3, Admin=4
            // Old: None=0, User=1, Client=2, Staff=3, Admin=4
            migrationBuilder.Sql(@"
                UPDATE UserProfiles 
                SET Role = 10 
                WHERE Role = 2;
                
                UPDATE UserProfiles 
                SET Role = 2 
                WHERE Role = 10;
            ");

            // Revert AgencyRole enum values
            // New: Agent=1, Manager=2, Owner=3
            // Old: Member=0, Agent=1, Manager=2, Owner=3
            // Note: Cannot accurately revert Agent (1) back to Member (0) or Agent (1)
            // Assuming all Agent (1) stay as Agent (1)

            // Revert StudioRole enum values
            // New: Staff=1, Manager=2, Owner=3
            // Old: Member=0, Editor=1, Photographer=2, Manager=3, Owner=4
            migrationBuilder.Sql(@"
                UPDATE Staff 
                SET Role = CASE 
                    WHEN Role = 1 THEN 0  -- Staff -> Member (default)
                    WHEN Role = 2 THEN 3  -- Manager -> Manager
                    WHEN Role = 3 THEN 4  -- Owner -> Owner
                    ELSE Role
                END;
            ");
        }
    }
}
