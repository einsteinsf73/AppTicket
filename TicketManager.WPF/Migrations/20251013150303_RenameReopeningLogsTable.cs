using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class RenameReopeningLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_REOPENING_LOGS_TICKETS_TICKET_ID",
                table: "REOPENING_LOGS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_REOPENING_LOGS",
                table: "REOPENING_LOGS");

            migrationBuilder.RenameTable(
                name: "REOPENING_LOGS",
                newName: "HIST_REABERTURA");

            migrationBuilder.RenameIndex(
                name: "IX_REOPENING_LOGS_TICKET_ID",
                table: "HIST_REABERTURA",
                newName: "IX_HIST_REABERTURA_TICKET_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HIST_REABERTURA",
                table: "HIST_REABERTURA",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_HIST_REABERTURA_TICKETS_TICKET_ID",
                table: "HIST_REABERTURA",
                column: "TICKET_ID",
                principalTable: "TICKETS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HIST_REABERTURA_TICKETS_TICKET_ID",
                table: "HIST_REABERTURA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HIST_REABERTURA",
                table: "HIST_REABERTURA");

            migrationBuilder.RenameTable(
                name: "HIST_REABERTURA",
                newName: "REOPENING_LOGS");

            migrationBuilder.RenameIndex(
                name: "IX_HIST_REABERTURA_TICKET_ID",
                table: "REOPENING_LOGS",
                newName: "IX_REOPENING_LOGS_TICKET_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_REOPENING_LOGS",
                table: "REOPENING_LOGS",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_REOPENING_LOGS_TICKETS_TICKET_ID",
                table: "REOPENING_LOGS",
                column: "TICKET_ID",
                principalTable: "TICKETS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
