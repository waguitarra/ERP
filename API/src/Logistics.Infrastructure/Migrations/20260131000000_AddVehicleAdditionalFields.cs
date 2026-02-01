using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Financial and identification fields
            migrationBuilder.AddColumn<string>(
                name: "ChassisNumber",
                table: "Vehicles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EngineNumber",
                table: "Vehicles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Vehicles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentValue",
                table: "Vehicles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            // Mileage fields
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentMileage",
                table: "Vehicles",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDistanceTraveled",
                table: "Vehicles",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // Insurance and documentation dates
            migrationBuilder.AddColumn<DateTime>(
                name: "InsuranceExpiryDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastInspectionDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextInspectionDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            // Maintenance tracking
            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastMaintenanceMileage",
                table: "Vehicles",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalMaintenanceCost",
                table: "Vehicles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ChassisNumber", table: "Vehicles");
            migrationBuilder.DropColumn(name: "EngineNumber", table: "Vehicles");
            migrationBuilder.DropColumn(name: "PurchasePrice", table: "Vehicles");
            migrationBuilder.DropColumn(name: "PurchaseDate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "CurrentValue", table: "Vehicles");
            migrationBuilder.DropColumn(name: "CurrentMileage", table: "Vehicles");
            migrationBuilder.DropColumn(name: "TotalDistanceTraveled", table: "Vehicles");
            migrationBuilder.DropColumn(name: "InsuranceExpiryDate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LicenseExpiryDate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LastInspectionDate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "NextInspectionDate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LastMaintenanceDate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LastMaintenanceMileage", table: "Vehicles");
            migrationBuilder.DropColumn(name: "TotalMaintenanceCost", table: "Vehicles");
        }
    }
}
