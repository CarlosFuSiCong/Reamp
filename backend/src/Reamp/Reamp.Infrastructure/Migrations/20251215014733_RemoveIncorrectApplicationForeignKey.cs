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
            // Remove incorrect foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationApplications_UserProfiles_ApplicantUserId",
                table: "OrganizationApplications");
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
