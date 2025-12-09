using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refactor_Slug_ValueObject_All : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.Id);
                    table.CheckConstraint("CK_Agencies_ContactEmail_NotEmpty", "LEN(LTRIM(RTRIM([ContactEmail]))) > 0");
                    table.CheckConstraint("CK_Agencies_ContactPhone_NotEmpty", "LEN(LTRIM(RTRIM([ContactPhone]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Address_Line1 = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address_Line2 = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Address_State = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Address_Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address_Country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Address_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    Address_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.Id);
                    table.CheckConstraint("CK_Studios_Address_Lat", "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");
                    table.CheckConstraint("CK_Studios_Address_Lng", "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_Studios_ContactEmail_NotEmpty", "LEN(LTRIM(RTRIM([ContactEmail]))) > 0");
                    table.CheckConstraint("CK_Studios_ContactPhone_NotEmpty", "LEN(LTRIM(RTRIM([ContactPhone]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "AgencyBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Address_Line1 = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address_Line2 = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Address_State = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Address_Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address_Country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Address_Latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    Address_Longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyBranches", x => x.Id);
                    table.CheckConstraint("CK_AgencyBranches_Address_Lat", "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");
                    table.CheckConstraint("CK_AgencyBranches_Address_Lng", "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_AgencyBranches_ContactEmail_NotEmpty", "LEN(LTRIM(RTRIM([ContactEmail]))) > 0");
                    table.CheckConstraint("CK_AgencyBranches_ContactPhone_NotEmpty", "LEN(LTRIM(RTRIM([ContactPhone]))) > 0");
                    table.ForeignKey(
                        name: "FK_AgencyBranches_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Skills = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                    table.CheckConstraint("CK_Staffs_Skills_Valid", "[Skills] >= 0");
                    table.ForeignKey(
                        name: "FK_Staffs_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Staffs_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_AgencyBranches_AgencyBranchId",
                        column: x => x.AgencyBranchId,
                        principalTable: "AgencyBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_CreatedBy",
                table: "Agencies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_Slug",
                table: "Agencies",
                column: "Slug",
                unique: true,
                filter: "[DeletedAtUtc] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyBranches_Address_City",
                table: "AgencyBranches",
                column: "Address_City");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyBranches_AgencyId_Slug",
                table: "AgencyBranches",
                columns: new[] { "AgencyId", "Slug" },
                unique: true,
                filter: "[DeletedAtUtc] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyBranches_CreatedBy",
                table: "AgencyBranches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AgencyBranchId",
                table: "Clients",
                column: "AgencyBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AgencyId",
                table: "Clients",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserProfileId",
                table: "Clients",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_StudioId",
                table: "Staffs",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserProfileId",
                table: "Staffs",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Studios_Address_City",
                table: "Studios",
                column: "Address_City");

            migrationBuilder.CreateIndex(
                name: "IX_Studios_CreatedBy",
                table: "Studios",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Studios_Slug",
                table: "Studios",
                column: "Slug",
                unique: true,
                filter: "[DeletedAtUtc] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "AgencyBranches");

            migrationBuilder.DropTable(
                name: "Studios");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);
        }
    }
}
