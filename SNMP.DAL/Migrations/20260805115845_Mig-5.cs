using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snmp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PollingEnabled",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PollingIntervalSeconds",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PollingEnabled",
                table: "Devices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PollingIntervalSeconds",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
