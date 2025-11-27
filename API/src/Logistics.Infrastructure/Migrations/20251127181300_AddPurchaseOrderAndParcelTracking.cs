using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderAndParcelTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Companies_CompanyId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Drivers_DriverId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Orders_OrderId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Suppliers_SupplierId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Vehicles_VehicleId",
                table: "InboundShipments");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualArrivalPort",
                table: "Orders",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillOfLading",
                table: "Orders",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CartonsPerParcel",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "Orders",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CustomsBroker",
                table: "Orders",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "DesiredMarginPercentage",
                table: "Orders",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DockDoorNumber",
                table: "Orders",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrivalPort",
                table: "Orders",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedProfit",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedCartons",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedParcels",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImportLicenseNumber",
                table: "Orders",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Incoterm",
                table: "Orders",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsInternational",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwnCarrier",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OriginCountry",
                table: "Orders",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PortOfEntry",
                table: "Orders",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ReceivedParcels",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingDistance",
                table: "Orders",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "SuggestedSalePrice",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "Orders",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyCarrier",
                table: "Orders",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UnitsPerCarton",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantityReceived",
                table: "InboundShipments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantityExpected",
                table: "InboundShipments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InboundShipments",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ShipmentNumber",
                table: "InboundShipments",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DockDoorNumber",
                table: "InboundShipments",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ASNNumber",
                table: "InboundShipments",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InboundParcels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InboundShipmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParcelNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LPN = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    TotalParcels = table.Column<int>(type: "int", nullable: false),
                    ParentParcelId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DimensionUnit = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrentLocation = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceivedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReceivedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    HasDamage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DamageNotes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundParcels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundParcels_InboundParcels_ParentParcelId",
                        column: x => x.ParentParcelId,
                        principalTable: "InboundParcels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InboundParcels_InboundShipments_InboundShipmentId",
                        column: x => x.InboundShipmentId,
                        principalTable: "InboundShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FileName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDocuments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InboundCartons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InboundParcelId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CartonNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Barcode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    TotalCartons = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceivedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReceivedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    HasDamage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DamageNotes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundCartons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundCartons_InboundParcels_InboundParcelId",
                        column: x => x.InboundParcelId,
                        principalTable: "InboundParcels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InboundParcelItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InboundParcelId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SKU = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpectedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundParcelItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundParcelItems_InboundParcels_InboundParcelId",
                        column: x => x.InboundParcelId,
                        principalTable: "InboundParcels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboundParcelItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InboundCartonItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InboundCartonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SKU = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SerialNumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsReceived = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReceivedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundCartonItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundCartonItems_InboundCartons_InboundCartonId",
                        column: x => x.InboundCartonId,
                        principalTable: "InboundCartons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboundCartonItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ContainerNumber",
                table: "Orders",
                column: "ContainerNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsInternational",
                table: "Orders",
                column: "IsInternational");

            migrationBuilder.CreateIndex(
                name: "IX_InboundShipments_ASNNumber",
                table: "InboundShipments",
                column: "ASNNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InboundShipments_ShipmentNumber",
                table: "InboundShipments",
                column: "ShipmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundShipments_Status",
                table: "InboundShipments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartonItems_InboundCartonId",
                table: "InboundCartonItems",
                column: "InboundCartonId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartonItems_ProductId",
                table: "InboundCartonItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartonItems_SerialNumber",
                table: "InboundCartonItems",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartonItems_SKU",
                table: "InboundCartonItems",
                column: "SKU");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartons_Barcode",
                table: "InboundCartons",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartons_InboundParcelId",
                table: "InboundCartons",
                column: "InboundParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundCartons_Status",
                table: "InboundCartons",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcelItems_InboundParcelId",
                table: "InboundParcelItems",
                column: "InboundParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcelItems_ProductId",
                table: "InboundParcelItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcelItems_SKU",
                table: "InboundParcelItems",
                column: "SKU");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcels_InboundShipmentId",
                table: "InboundParcels",
                column: "InboundShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcels_LPN",
                table: "InboundParcels",
                column: "LPN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcels_ParcelNumber",
                table: "InboundParcels",
                column: "ParcelNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcels_ParentParcelId",
                table: "InboundParcels",
                column: "ParentParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundParcels_Status",
                table: "InboundParcels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDocuments_OrderId",
                table: "OrderDocuments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDocuments_Type",
                table: "OrderDocuments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDocuments_UploadedAt",
                table: "OrderDocuments",
                column: "UploadedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Companies_CompanyId",
                table: "InboundShipments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Drivers_DriverId",
                table: "InboundShipments",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Orders_OrderId",
                table: "InboundShipments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Suppliers_SupplierId",
                table: "InboundShipments",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Vehicles_VehicleId",
                table: "InboundShipments",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Companies_CompanyId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Drivers_DriverId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Orders_OrderId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Suppliers_SupplierId",
                table: "InboundShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_InboundShipments_Vehicles_VehicleId",
                table: "InboundShipments");

            migrationBuilder.DropTable(
                name: "InboundCartonItems");

            migrationBuilder.DropTable(
                name: "InboundParcelItems");

            migrationBuilder.DropTable(
                name: "OrderDocuments");

            migrationBuilder.DropTable(
                name: "InboundCartons");

            migrationBuilder.DropTable(
                name: "InboundParcels");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ContainerNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_IsInternational",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_InboundShipments_ASNNumber",
                table: "InboundShipments");

            migrationBuilder.DropIndex(
                name: "IX_InboundShipments_ShipmentNumber",
                table: "InboundShipments");

            migrationBuilder.DropIndex(
                name: "IX_InboundShipments_Status",
                table: "InboundShipments");

            migrationBuilder.DropColumn(
                name: "ActualArrivalPort",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillOfLading",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CartonsPerParcel",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomsBroker",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DesiredMarginPercentage",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DockDoorNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EstimatedArrivalPort",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EstimatedProfit",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExpectedCartons",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExpectedParcels",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ImportLicenseNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Incoterm",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsInternational",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsOwnCarrier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OriginCountry",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PortOfEntry",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReceivedParcels",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDistance",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SuggestedSalePrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ThirdPartyCarrier",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UnitsPerCarton",
                table: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantityReceived",
                table: "InboundShipments",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantityExpected",
                table: "InboundShipments",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InboundShipments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ShipmentNumber",
                table: "InboundShipments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DockDoorNumber",
                table: "InboundShipments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ASNNumber",
                table: "InboundShipments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Companies_CompanyId",
                table: "InboundShipments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Drivers_DriverId",
                table: "InboundShipments",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Orders_OrderId",
                table: "InboundShipments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Suppliers_SupplierId",
                table: "InboundShipments",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InboundShipments_Vehicles_VehicleId",
                table: "InboundShipments",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }
    }
}
