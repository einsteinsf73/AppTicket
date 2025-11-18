using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetMaintenanceAndLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MANUTENCAO",
                table: "PATRIMONIO_HISTORICO",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PREVISAO_MANUTENCAO",
                table: "PATRIMONIO_HISTORICO",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MANUTENCAO",
                table: "PATRIMONIO",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PREVISAO_MANUTENCAO",
                table: "PATRIMONIO",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PATRIMONIO_LOGS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ORIGINAL_ASSET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ITEM = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    BRAND = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MODEL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SERIAL_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ASSET_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    VALUE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PURCHASE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SUPPLIER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    INVOICE_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    WARRANTY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    LOCATION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SECTOR = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DEPARTMENT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMPLOYEE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MANUTENCAO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PREVISAO_MANUTENCAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    OBSERVATIONS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DELETED_BY_HOSTNAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DELETED_BY_WINDOWS_USER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DELETION_TIMESTAMP = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATRIMONIO_LOGS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PATRIMONIO_LOGS");

            migrationBuilder.DropColumn(
                name: "MANUTENCAO",
                table: "PATRIMONIO_HISTORICO");

            migrationBuilder.DropColumn(
                name: "PREVISAO_MANUTENCAO",
                table: "PATRIMONIO_HISTORICO");

            migrationBuilder.DropColumn(
                name: "MANUTENCAO",
                table: "PATRIMONIO");

            migrationBuilder.DropColumn(
                name: "PREVISAO_MANUTENCAO",
                table: "PATRIMONIO");
        }
    }
}
