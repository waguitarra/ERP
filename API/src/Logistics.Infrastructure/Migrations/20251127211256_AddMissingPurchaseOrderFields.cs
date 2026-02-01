using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingPurchaseOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Esta migration foi originalmente criada para adicionar campos que já existem
            // na migration SeparatePurchaseAndSalesOrders. Mantida vazia para compatibilidade.
            // Os campos DestinationWarehouseId, DriverId, VehicleId já existem.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nada a reverter - campos já existiam na migration anterior
        }
    }
}
