using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reamp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeStudioIdAndPhotographerIdOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "StudioId",
                table: "ShootOrders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ShootOrders_AssignedPhotographerId",
                table: "ShootOrders",
                column: "AssignedPhotographerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShootOrders_Staffs_AssignedPhotographerId",
                table: "ShootOrders",
                column: "AssignedPhotographerId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShootOrders_Staffs_AssignedPhotographerId",
                table: "ShootOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShootOrders_AssignedPhotographerId",
                table: "ShootOrders");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudioId",
                table: "ShootOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
