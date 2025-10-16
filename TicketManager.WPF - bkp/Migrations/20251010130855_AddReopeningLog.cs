using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class AddReopeningLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REOPENING_LOGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TicketId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ReopenedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ReopenedBy = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Reason = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REOPENING_LOGS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_REOPENING_LOGS_TICKETS_TicketId",
                        column: x => x.TicketId,
                        principalTable: "TICKETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_REOPENING_LOGS_TicketId",
                table: "REOPENING_LOGS",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REOPENING_LOGS");
        }
    }
}
