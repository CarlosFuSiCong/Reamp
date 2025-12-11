using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Allow_Optional_LastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UserProfiles_LastName_NotEmpty",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "UserProfiles",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedPhotographerId",
                table: "ShootOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledEndUtc",
                table: "ShootOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartUtc",
                table: "ShootOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchedulingNotes",
                table: "ShootOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedPhotographerId",
                table: "ShootOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledEndUtc",
                table: "ShootOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledStartUtc",
                table: "ShootOrders");

            migrationBuilder.DropColumn(
                name: "SchedulingNotes",
                table: "ShootOrders");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "UserProfiles",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserProfiles_LastName_NotEmpty",
                table: "UserProfiles",
                sql: "LEN(LTRIM(RTRIM([LastName]))) > 0");
        }
    }
}
