using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HAS_ASSET_ACCESS",
                table: "AUTHORIZED_USERS",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HAS_TICKET_ACCESS",
                table: "AUTHORIZED_USERS",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HAS_ASSET_ACCESS",
                table: "AUTHORIZED_USERS");

            migrationBuilder.DropColumn(
                name: "HAS_TICKET_ACCESS",
                table: "AUTHORIZED_USERS");
        }
    }
}
