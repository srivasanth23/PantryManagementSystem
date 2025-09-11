using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PantryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialPantry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Billings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PantryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PantryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PantryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_PantryItems_PantryItemId",
                        column: x => x.PantryItemId,
                        principalTable: "PantryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Billings",
                columns: new[] { "Id", "GeneratedDate", "Month", "TotalAmount", "UserId" },
                values: new object[] { new Guid("c8f2e7c2-3af8-4a8a-91d1-28e5f5ffdb92"), new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sep-2025", 40m, new Guid("20e32654-1f4f-4e36-8c1d-7a805d5218dd") });

            migrationBuilder.InsertData(
                table: "PantryItems",
                columns: new[] { "Id", "Category", "ExpiryDate", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { new Guid("91555176-1298-4383-8e3e-bb0695861ee7"), "Beverage", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Coffee Powder", 20m, 50 },
                    { new Guid("aec40d63-08c4-42b5-92e6-f5cc46187222"), "Beverage", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tea Powder", 10m, 100 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "IssuedDate", "PantryItemId", "Quantity", "RequestDate", "Status", "UserId" },
                values: new object[] { new Guid("aca1f837-fba7-42d2-ae01-7f25ccfc1653"), new DateTime(2025, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("91555176-1298-4383-8e3e-bb0695861ee7"), 2, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Issued", new Guid("20e32654-1f4f-4e36-8c1d-7a805d5218dd") });

            migrationBuilder.CreateIndex(
                name: "IX_Billings_UserId_Month",
                table: "Billings",
                columns: new[] { "UserId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PantryItemId",
                table: "Orders",
                column: "PantryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Billings");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PantryItems");
        }
    }
}
