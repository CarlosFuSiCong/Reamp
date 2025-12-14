using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRoleConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Staffs_Role_Valid",
                table: "Staffs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Agents_Role_Valid",
                table: "Agents");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Staffs_Role_Valid",
                table: "Staffs",
                sql: "[Role] >= 1 AND [Role] <= 3");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Agents_Role_Valid",
                table: "Agents",
                sql: "[Role] >= 1 AND [Role] <= 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Staffs_Role_Valid",
                table: "Staffs");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Agents_Role_Valid",
                table: "Agents");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Staffs_Role_Valid",
                table: "Staffs",
                sql: "[Role] >= 0 AND [Role] <= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Agents_Role_Valid",
                table: "Agents",
                sql: "[Role] >= 0 AND [Role] <= 3");
        }
    }
}
