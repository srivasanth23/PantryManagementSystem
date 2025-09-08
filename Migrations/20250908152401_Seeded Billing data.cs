using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PantryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeededBillingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Billings",
                columns: new[] { "Id", "GeneratedDate", "Month", "TotalAmount", "UserId" },
                values: new object[] { new Guid("c8f2e7c2-3af8-4a8a-91d1-28e5f5ffdb92"), new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sep-2025", 40m, new Guid("20e32654-1f4f-4e36-8c1d-7a805d5218dd") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Billings",
                keyColumn: "Id",
                keyValue: new Guid("c8f2e7c2-3af8-4a8a-91d1-28e5f5ffdb92"));
        }
    }
}
