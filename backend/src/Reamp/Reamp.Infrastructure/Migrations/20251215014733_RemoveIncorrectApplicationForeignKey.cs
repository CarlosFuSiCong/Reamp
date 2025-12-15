using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIncorrectApplicationForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if constraint exists before dropping
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrganizationApplications_UserProfiles_ApplicantUserId')
                BEGIN
                    ALTER TABLE [OrganizationApplications] DROP CONSTRAINT [FK_OrganizationApplications_UserProfiles_ApplicantUserId];
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add the foreign key if rolling back (not recommended)
            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationApplications_UserProfiles_ApplicantUserId",
                table: "OrganizationApplications",
                column: "ApplicantUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
