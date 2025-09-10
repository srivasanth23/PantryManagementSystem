using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PantryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Guidfororderchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("aca1f837-fba7-42d2-ae01-7f25ccfc1653"),
                column: "UserId",
                value: new Guid("20e32654-1f4f-4e36-8c1d-7a805d5218dd"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: new Guid("aca1f837-fba7-42d2-ae01-7f25ccfc1653"),
                column: "UserId",
                value: "20e32654-1f4f-4e36-8c1d-7a805d5218dd");
        }
    }
}
