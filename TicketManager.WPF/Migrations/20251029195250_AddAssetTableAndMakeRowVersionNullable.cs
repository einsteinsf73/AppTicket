using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetTableAndMakeRowVersionNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ROW_VERSION",
                table: "TICKETS",
                type: "RAW(8)",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ASSETS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
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
                    OBSERVATIONS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSETS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ASSETS");

            migrationBuilder.DropColumn(
                name: "ROW_VERSION",
                table: "TICKETS");
        }
    }
}
