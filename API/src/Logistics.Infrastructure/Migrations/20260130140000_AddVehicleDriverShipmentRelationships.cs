using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleDriverShipmentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add DriverId to Vehicles (FK to Drivers)
            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "Vehicles",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            // Add CurrentShipmentId to Vehicles (FK to OutboundShipments)
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentShipmentId",
                table: "Vehicles",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            // Add IsMoving to Vehicles
            migrationBuilder.AddColumn<bool>(
                name: "IsMoving",
                table: "Vehicles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Add VehicleId to OutboundShipments
            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "OutboundShipments",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_DriverId",
                table: "Vehicles",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CurrentShipmentId",
                table: "Vehicles",
                column: "CurrentShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundShipments_VehicleId",
                table: "OutboundShipments",
                column: "VehicleId");

            // Add foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_DriverId",
                table: "Vehicles",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_OutboundShipments_CurrentShipmentId",
                table: "Vehicles",
                column: "CurrentShipmentId",
                principalTable: "OutboundShipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OutboundShipments_Vehicles_VehicleId",
                table: "OutboundShipments",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboundShipments_Vehicles_VehicleId",
                table: "OutboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_OutboundShipments_CurrentShipmentId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Drivers_DriverId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_OutboundShipments_VehicleId",
                table: "OutboundShipments");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CurrentShipmentId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_DriverId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "OutboundShipments");

            migrationBuilder.DropColumn(
                name: "IsMoving",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CurrentShipmentId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Vehicles");
        }
    }
}
