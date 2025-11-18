using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PATRIMONIO_HISTORICO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ASSET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ITEM = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    BRAND = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MODEL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SERIAL_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ASSET_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    VALUE = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false),
                    PURCHASE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SUPPLIER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    INVOICE_NUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    WARRANTY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    LOCATION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SECTOR = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DEPARTMENT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMPLOYEE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    OBSERVATIONS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MODIFIED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MODIFIED_BY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATRIMONIO_HISTORICO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PATRIMONIO_HISTORICO_PATRIMONIO_ASSET_ID",
                        column: x => x.ASSET_ID,
                        principalTable: "PATRIMONIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PATRIMONIO_HISTORICO_ASSET_ID",
                table: "PATRIMONIO_HISTORICO",
                column: "ASSET_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PATRIMONIO_HISTORICO");
        }
    }
}
