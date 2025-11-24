using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_RefreshTokens_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staffs_UserProfileId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UserProfileId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Agencies");

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarAssetId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: true);

            // Drop check constraints before altering columns
            migrationBuilder.DropCheckConstraint(
                name: "CK_Studios_Address_Lat",
                table: "Studios");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Studios_Address_Lng",
                table: "Studios");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AgencyBranches_Address_Lat",
                table: "AgencyBranches");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AgencyBranches_Address_Lng",
                table: "AgencyBranches");

            migrationBuilder.AlterColumn<double>(
                name: "Address_Longitude",
                table: "Studios",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Address_Latitude",
                table: "Studios",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LogoAssetId",
                table: "Studios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Address_Longitude",
                table: "AgencyBranches",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Address_Latitude",
                table: "AgencyBranches",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            // Recreate check constraints after altering columns
            migrationBuilder.AddCheckConstraint(
                name: "CK_Studios_Address_Lat",
                table: "Studios",
                sql: "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Studios_Address_Lng",
                table: "Studios",
                sql: "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AgencyBranches_Address_Lat",
                table: "AgencyBranches",
                sql: "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AgencyBranches_Address_Lng",
                table: "AgencyBranches",
                sql: "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");

            migrationBuilder.AddColumn<Guid>(
                name: "LogoAssetId",
                table: "Agencies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerAgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ListingType = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AvailableFromUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<int>(type: "int", nullable: false),
                    ParkingSpaces = table.Column<int>(type: "int", nullable: false),
                    FloorAreaSqm = table.Column<double>(type: "float", nullable: true),
                    LandAreaSqm = table.Column<double>(type: "float", nullable: true),
                    Address_Line1 = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Address_Line2 = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Address_State = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Address_Postcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address_Country = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Address_Latitude = table.Column<double>(type: "float", nullable: true),
                    Address_Longitude = table.Column<double>(type: "float", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.Id);
                    table.CheckConstraint("CK_Listings_Price_Positive", "[Price] > 0");
                    table.ForeignKey(
                        name: "FK_Listings_Agencies_OwnerAgencyId",
                        column: x => x.OwnerAgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerStudioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploaderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    ProviderAssetId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ResourceType = table.Column<int>(type: "int", nullable: false),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    WidthPx = table.Column<int>(type: "int", nullable: true),
                    HeightPx = table.Column<int>(type: "int", nullable: true),
                    DurationSeconds = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PublicUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ChecksumSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssets", x => x.Id);
                    table.CheckConstraint("CK_MediaAssets_Duration_Positive", "[DurationSeconds] IS NULL OR [DurationSeconds] > 0");
                    table.CheckConstraint("CK_MediaAssets_Size_Positive", "[SizeBytes] > 0");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListingAgentSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    TeamOrOfficeName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAgentSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingAgentSnapshots_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShootOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShootOrders", x => x.Id);
                    table.CheckConstraint("CK_ShootOrders_Total_NotNegative", "[TotalAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_ShootOrders_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShootOrders_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShootOrders_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListingMediaRefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsCover = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingMediaRefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingMediaRefs_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListingMediaRefs_MediaAssets_MediaAssetId",
                        column: x => x.MediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MediaVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    WidthPx = table.Column<int>(type: "int", nullable: true),
                    HeightPx = table.Column<int>(type: "int", nullable: true),
                    BitrateKbps = table.Column<int>(type: "int", nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaVariants", x => x.Id);
                    table.CheckConstraint("CK_MediaVariants_Size_Positive", "[SizeBytes] IS NULL OR [SizeBytes] > 0");
                    table.ForeignKey(
                        name: "FK_MediaVariants_MediaAssets_MediaAssetId",
                        column: x => x.MediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WatermarkEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPackages", x => x.Id);
                    table.CheckConstraint("CK_DeliveryPackages_Title_NotEmpty", "LEN(LTRIM(RTRIM([Title]))) > 0");
                    table.ForeignKey(
                        name: "FK_DeliveryPackages_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryPackages_ShootOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ShootOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShootTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShootOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssigneeUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScheduledStartUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledEndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShootTasks", x => x.Id);
                    table.CheckConstraint("CK_ShootTasks_Price_Positive", "[Price] IS NULL OR [Price] > 0");
                    table.CheckConstraint("CK_ShootTasks_Time", "[ScheduledStartUtc] IS NULL OR [ScheduledEndUtc] IS NULL OR [ScheduledEndUtc] > [ScheduledStartUtc]");
                    table.CheckConstraint("CK_ShootTasks_Type_NonNegative", "[Type] >= 0");
                    table.ForeignKey(
                        name: "FK_ShootTasks_ShootOrders_ShootOrderId",
                        column: x => x.ShootOrderId,
                        principalTable: "ShootOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    MaxDownloads = table.Column<int>(type: "int", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Downloads = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAccess", x => x.Id);
                    table.CheckConstraint("CK_DeliveryAccess_Downloads_NonNegative", "[Downloads] >= 0");
                    table.CheckConstraint("CK_DeliveryAccess_MaxDownloads_Positive", "[MaxDownloads] IS NULL OR [MaxDownloads] > 0");
                    table.ForeignKey(
                        name: "FK_DeliveryAccess_DeliveryPackages_DeliveryPackageId",
                        column: x => x.DeliveryPackageId,
                        principalTable: "DeliveryPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_DeliveryPackages_DeliveryPackageId",
                        column: x => x.DeliveryPackageId,
                        principalTable: "DeliveryPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_MediaAssets_MediaAssetId",
                        column: x => x.MediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_AvatarAssetId",
                table: "UserProfiles",
                column: "AvatarAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Studios_LogoAssetId",
                table: "Studios",
                column: "LogoAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserProfileId",
                table: "Staffs",
                column: "UserProfileId",
                unique: true,
                filter: "[DeletedAtUtc] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserProfileId",
                table: "Clients",
                column: "UserProfileId",
                unique: true,
                filter: "[DeletedAtUtc] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_LogoAssetId",
                table: "Agencies",
                column: "LogoAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAccess_DeliveryPackageId",
                table: "DeliveryAccess",
                column: "DeliveryPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAccess_DeliveryPackageId_Type",
                table: "DeliveryAccess",
                columns: new[] { "DeliveryPackageId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_DeliveryPackageId",
                table: "DeliveryItems",
                column: "DeliveryPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_DeliveryPackageId_MediaAssetId_VariantName",
                table: "DeliveryItems",
                columns: new[] { "DeliveryPackageId", "MediaAssetId", "VariantName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_MediaAssetId",
                table: "DeliveryItems",
                column: "MediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackages_ExpiresAtUtc",
                table: "DeliveryPackages",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackages_ListingId",
                table: "DeliveryPackages",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackages_OrderId",
                table: "DeliveryPackages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPackages_Status",
                table: "DeliveryPackages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAgentSnapshots_ListingId",
                table: "ListingAgentSnapshots",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAgentSnapshots_ListingId_IsPrimary",
                table: "ListingAgentSnapshots",
                columns: new[] { "ListingId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ListingMediaRefs_ListingId",
                table: "ListingMediaRefs",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingMediaRefs_ListingId_IsCover",
                table: "ListingMediaRefs",
                columns: new[] { "ListingId", "IsCover" },
                unique: true,
                filter: "[IsCover] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ListingMediaRefs_ListingId_Role_SortOrder",
                table: "ListingMediaRefs",
                columns: new[] { "ListingId", "Role", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListingMediaRefs_MediaAssetId",
                table: "ListingMediaRefs",
                column: "MediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_AgentUserId",
                table: "Listings",
                column: "AgentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_OwnerAgencyId",
                table: "Listings",
                column: "OwnerAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Price",
                table: "Listings",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_PropertyType",
                table: "Listings",
                column: "PropertyType");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_Status_ListingType",
                table: "Listings",
                columns: new[] { "Status", "ListingType" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_OwnerStudioId",
                table: "MediaAssets",
                column: "OwnerStudioId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_OwnerStudioId_ChecksumSha256",
                table: "MediaAssets",
                columns: new[] { "OwnerStudioId", "ChecksumSha256" },
                unique: true,
                filter: "[ChecksumSha256] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_ProcessStatus",
                table: "MediaAssets",
                column: "ProcessStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Provider_ProviderAssetId",
                table: "MediaAssets",
                columns: new[] { "Provider", "ProviderAssetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaVariants_MediaAssetId",
                table: "MediaVariants",
                column: "MediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaVariants_MediaAssetId_Name",
                table: "MediaVariants",
                columns: new[] { "MediaAssetId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_IsRevoked_ExpiresAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "IsRevoked", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ShootOrders_AgencyId",
                table: "ShootOrders",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShootOrders_ListingId",
                table: "ShootOrders",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShootOrders_Status",
                table: "ShootOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ShootOrders_StudioId",
                table: "ShootOrders",
                column: "StudioId");

            migrationBuilder.CreateIndex(
                name: "IX_ShootTasks_ShootOrderId",
                table: "ShootTasks",
                column: "ShootOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShootTasks_Type_Status",
                table: "ShootTasks",
                columns: new[] { "Type", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_MediaAssets_LogoAssetId",
                table: "Agencies",
                column: "LogoAssetId",
                principalTable: "MediaAssets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Studios_MediaAssets_LogoAssetId",
                table: "Studios",
                column: "LogoAssetId",
                principalTable: "MediaAssets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_MediaAssets_AvatarAssetId",
                table: "UserProfiles",
                column: "AvatarAssetId",
                principalTable: "MediaAssets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_MediaAssets_LogoAssetId",
                table: "Agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Studios_MediaAssets_LogoAssetId",
                table: "Studios");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_MediaAssets_AvatarAssetId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "DeliveryAccess");

            migrationBuilder.DropTable(
                name: "DeliveryItems");

            migrationBuilder.DropTable(
                name: "ListingAgentSnapshots");

            migrationBuilder.DropTable(
                name: "ListingMediaRefs");

            migrationBuilder.DropTable(
                name: "MediaVariants");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ShootTasks");

            migrationBuilder.DropTable(
                name: "DeliveryPackages");

            migrationBuilder.DropTable(
                name: "MediaAssets");

            migrationBuilder.DropTable(
                name: "ShootOrders");

            migrationBuilder.DropTable(
                name: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_AvatarAssetId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Studios_LogoAssetId",
                table: "Studios");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_UserProfileId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UserProfileId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Agencies_LogoAssetId",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "AvatarAssetId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LogoAssetId",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "LogoAssetId",
                table: "Agencies");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "UserProfiles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            // Drop check constraints before altering columns
            migrationBuilder.DropCheckConstraint(
                name: "CK_Studios_Address_Lat",
                table: "Studios");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Studios_Address_Lng",
                table: "Studios");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AgencyBranches_Address_Lat",
                table: "AgencyBranches");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AgencyBranches_Address_Lng",
                table: "AgencyBranches");

            migrationBuilder.AlterColumn<double>(
                name: "Address_Longitude",
                table: "Studios",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Address_Latitude",
                table: "Studios",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Studios",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Address_Longitude",
                table: "AgencyBranches",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Address_Latitude",
                table: "AgencyBranches",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            // Recreate check constraints after altering columns
            migrationBuilder.AddCheckConstraint(
                name: "CK_Studios_Address_Lat",
                table: "Studios",
                sql: "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Studios_Address_Lng",
                table: "Studios",
                sql: "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AgencyBranches_Address_Lat",
                table: "AgencyBranches",
                sql: "[Address_Latitude] IS NULL OR ([Address_Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AgencyBranches_Address_Lng",
                table: "AgencyBranches",
                sql: "[Address_Longitude] IS NULL OR ([Address_Longitude] BETWEEN -180 AND 180)");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Agencies",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserProfileId",
                table: "Staffs",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserProfileId",
                table: "Clients",
                column: "UserProfileId",
                unique: true);
        }
    }
}
