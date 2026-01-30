using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Vehicle basic fields
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Vehicles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "Vehicles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Capacity",
                table: "Vehicles",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Vehicles",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "Vehicles",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Vehicles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Tracking fields
            migrationBuilder.AddColumn<string>(
                name: "TrackingToken",
                table: "Vehicles",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "TrackingEnabled",
                table: "Vehicles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "LastLatitude",
                table: "Vehicles",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastLongitude",
                table: "Vehicles",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLocationUpdate",
                table: "Vehicles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentSpeed",
                table: "Vehicles",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentAddress",
                table: "Vehicles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Driver fields
            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "Vehicles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DriverPhone",
                table: "Vehicles",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Brand", table: "Vehicles");
            migrationBuilder.DropColumn(name: "VehicleType", table: "Vehicles");
            migrationBuilder.DropColumn(name: "Capacity", table: "Vehicles");
            migrationBuilder.DropColumn(name: "Color", table: "Vehicles");
            migrationBuilder.DropColumn(name: "FuelType", table: "Vehicles");
            migrationBuilder.DropColumn(name: "Notes", table: "Vehicles");
            migrationBuilder.DropColumn(name: "TrackingToken", table: "Vehicles");
            migrationBuilder.DropColumn(name: "TrackingEnabled", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LastLatitude", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LastLongitude", table: "Vehicles");
            migrationBuilder.DropColumn(name: "LastLocationUpdate", table: "Vehicles");
            migrationBuilder.DropColumn(name: "CurrentSpeed", table: "Vehicles");
            migrationBuilder.DropColumn(name: "CurrentAddress", table: "Vehicles");
            migrationBuilder.DropColumn(name: "DriverName", table: "Vehicles");
            migrationBuilder.DropColumn(name: "DriverPhone", table: "Vehicles");
        }
    }
}
