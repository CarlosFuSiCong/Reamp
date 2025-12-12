using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitationsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TargetEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InviteeEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    InviteeUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetRoleValue = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InvitedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.CheckConstraint("CK_Invitations_Status_Valid", "[Status] >= 0 AND [Status] <= 4");
                    table.CheckConstraint("CK_Invitations_Type_Valid", "[Type] >= 0 AND [Type] <= 1");
                    table.ForeignKey(
                        name: "FK_Invitations_UserProfiles_InvitedBy",
                        column: x => x.InvitedBy,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_InvitedBy",
                table: "Invitations",
                column: "InvitedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_InviteeEmail",
                table: "Invitations",
                column: "InviteeEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_InviteeEmail_Status_ExpiresAtUtc",
                table: "Invitations",
                columns: new[] { "InviteeEmail", "Status", "ExpiresAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_TargetEntityId_Type",
                table: "Invitations",
                columns: new[] { "TargetEntityId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitations");
        }
    }
}
