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
            // New: None=0, User=1, Agent=2, Staff=3, Admin=4
            // User (1) stays as 1 - no change needed
            // Client (2) -> Agent (2) - rename only, value unchanged
            
            // Note: No database updates needed for UserRole
            // Value 1 remains 1 (User), value 2 remains 2 (now Agent instead of Client)
            // This is a semantic rename only - Client role renamed to Agent

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

            // Drop old check constraint and create new one for AgencyRole
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Agents')
                BEGIN
                    ALTER TABLE Agents DROP CONSTRAINT IF EXISTS CK_Agents_Role_Valid;
                    ALTER TABLE Agents ADD CONSTRAINT CK_Agents_Role_Valid CHECK ([Role] >= 1 AND [Role] <= 3);
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

            // Drop old check constraint and create new one for StudioRole
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff')
                BEGIN
                    ALTER TABLE Staff DROP CONSTRAINT IF EXISTS CK_Staffs_Role_Valid;
                    ALTER TABLE Staff ADD CONSTRAINT CK_Staffs_Role_Valid CHECK ([Role] >= 1 AND [Role] <= 3);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert UserRole enum values
            // New: None=0, User=1, Agent=2, Staff=3, Admin=4
            // Old: None=0, User=1, Client=2, Staff=3, Admin=4
            // Note: No database changes needed - this was a semantic rename only
            // Agent (2) reverts to Client (2), User (1) remains User (1)

            // Revert AgencyRole check constraint (back to 0-3)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Agents')
                BEGIN
                    ALTER TABLE Agents DROP CONSTRAINT IF EXISTS CK_Agents_Role_Valid;
                    ALTER TABLE Agents ADD CONSTRAINT CK_Agents_Role_Valid CHECK ([Role] >= 0 AND [Role] <= 3);
                END
            ");

            // Revert AgencyRole enum values
            // New: Agent=1, Manager=2, Owner=3
            // Old: Member=0, Agent=1, Manager=2, Owner=3
            // Note: Cannot accurately revert Agent (1) back to Member (0) or Agent (1)
            // Assuming all Agent (1) stay as Agent (1)

            // Revert StudioRole check constraint (back to 0-4)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff')
                BEGIN
                    ALTER TABLE Staff DROP CONSTRAINT IF EXISTS CK_Staffs_Role_Valid;
                    ALTER TABLE Staff ADD CONSTRAINT CK_Staffs_Role_Valid CHECK ([Role] >= 0 AND [Role] <= 4);
                END
            ");

            // Revert StudioRole enum values
            // New: Staff=1, Manager=2, Owner=3
            // Old: Member=0, Editor=1, Photographer=2, Manager=3, Owner=4
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff')
                BEGIN
                    UPDATE Staff 
                    SET Role = CASE 
                        WHEN Role = 1 THEN 0  -- Staff -> Member (default)
                        WHEN Role = 2 THEN 3  -- Manager -> Manager
                        WHEN Role = 3 THEN 4  -- Owner -> Owner
                        ELSE Role
                    END;
                END
            ");
        }
    }
}
