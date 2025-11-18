using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class RenameAssetTableToPatrimonio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ASSETS",
                table: "ASSETS");

            migrationBuilder.RenameTable(
                name: "ASSETS",
                newName: "PATRIMONIO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PATRIMONIO",
                table: "PATRIMONIO",
                column: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PATRIMONIO",
                table: "PATRIMONIO");

            migrationBuilder.RenameTable(
                name: "PATRIMONIO",
                newName: "ASSETS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ASSETS",
                table: "ASSETS",
                column: "ID");
        }
    }
}
