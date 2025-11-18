using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ROW_VERSION",
                table: "TICKETS",
                type: "RAW(8)",
                rowVersion: true,
                nullable: true,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ROW_VERSION",
                table: "TICKETS");
        }
    }
}
