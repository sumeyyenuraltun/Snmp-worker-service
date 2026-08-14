using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Snmp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Mig11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 12, 7, 34, 23, 783, DateTimeKind.Utc).AddTicks(1904), true, "Admin", null },
                    { 2, new DateTime(2026, 8, 12, 7, 34, 23, 783, DateTimeKind.Utc).AddTicks(2828), true, "User", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
