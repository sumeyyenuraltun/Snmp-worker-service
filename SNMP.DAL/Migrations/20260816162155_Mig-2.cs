using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snmp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SnmpCredentials_DeviceId",
                table: "SnmpCredentials");

            migrationBuilder.CreateIndex(
                name: "IX_SnmpCredentials_DeviceId",
                table: "SnmpCredentials",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SnmpCredentials_DeviceId",
                table: "SnmpCredentials");

            migrationBuilder.CreateIndex(
                name: "IX_SnmpCredentials_DeviceId",
                table: "SnmpCredentials",
                column: "DeviceId",
                unique: true);
        }
    }
}
