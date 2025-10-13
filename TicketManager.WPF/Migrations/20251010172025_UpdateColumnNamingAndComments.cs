using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.WPF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNamingAndComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_REOPENING_LOGS_TICKETS_TicketId",
                table: "REOPENING_LOGS");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TICKETS",
                newName: "TITLE");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TICKETS",
                newName: "STATUS");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "TICKETS",
                newName: "PRIORITY");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TICKETS",
                newName: "DESCRIPTION");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TICKETS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "TICKETS",
                newName: "UPDATED_AT");

            migrationBuilder.RenameColumn(
                name: "SlaMinutes",
                table: "TICKETS",
                newName: "SLA_MINUTES");

            migrationBuilder.RenameColumn(
                name: "CreatedByWindowsUser",
                table: "TICKETS",
                newName: "CREATED_BY_WINDOWS_USER");

            migrationBuilder.RenameColumn(
                name: "CreatedByHostname",
                table: "TICKETS",
                newName: "CREATED_BY_HOSTNAME");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TICKETS",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TICKET_LOGS",
                newName: "TITLE");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TICKET_LOGS",
                newName: "STATUS");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "TICKET_LOGS",
                newName: "PRIORITY");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TICKET_LOGS",
                newName: "DESCRIPTION");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TICKET_LOGS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "TICKET_LOGS",
                newName: "UPDATED_AT");

            migrationBuilder.RenameColumn(
                name: "SlaMinutes",
                table: "TICKET_LOGS",
                newName: "SLA_MINUTES");

            migrationBuilder.RenameColumn(
                name: "OriginalTicketId",
                table: "TICKET_LOGS",
                newName: "ORIGINAL_TICKET_ID");

            migrationBuilder.RenameColumn(
                name: "DeletionTimestamp",
                table: "TICKET_LOGS",
                newName: "DELETION_TIMESTAMP");

            migrationBuilder.RenameColumn(
                name: "DeletedByWindowsUser",
                table: "TICKET_LOGS",
                newName: "DELETED_BY_WINDOWS_USER");

            migrationBuilder.RenameColumn(
                name: "DeletedByHostname",
                table: "TICKET_LOGS",
                newName: "DELETED_BY_HOSTNAME");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TICKET_LOGS",
                newName: "CREATED_AT");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "REOPENING_LOGS",
                newName: "REASON");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "REOPENING_LOGS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "REOPENING_LOGS",
                newName: "TICKET_ID");

            migrationBuilder.RenameColumn(
                name: "ReopenedBy",
                table: "REOPENING_LOGS",
                newName: "REOPENED_BY");

            migrationBuilder.RenameColumn(
                name: "ReopenedAt",
                table: "REOPENING_LOGS",
                newName: "REOPENED_AT");

            migrationBuilder.RenameIndex(
                name: "IX_REOPENING_LOGS_TicketId",
                table: "REOPENING_LOGS",
                newName: "IX_REOPENING_LOGS_TICKET_ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AUTHORIZED_USERS",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "WindowsUserName",
                table: "AUTHORIZED_USERS",
                newName: "WINDOWS_USER_NAME");

            migrationBuilder.RenameColumn(
                name: "IsAdmin",
                table: "AUTHORIZED_USERS",
                newName: "IS_ADMIN");

            migrationBuilder.AlterColumn<int>(
                name: "STATUS",
                table: "TICKETS",
                type: "NUMBER(10)",
                nullable: false,
                comment: "0=Aberto, 1=EmAndamento, 2=Resolvido, 3=Fechado",
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<int>(
                name: "PRIORITY",
                table: "TICKETS",
                type: "NUMBER(10)",
                nullable: false,
                comment: "0=Baixa, 1=Media, 2=Alta",
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AddForeignKey(
                name: "FK_REOPENING_LOGS_TICKETS_TICKET_ID",
                table: "REOPENING_LOGS",
                column: "TICKET_ID",
                principalTable: "TICKETS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_REOPENING_LOGS_TICKETS_TICKET_ID",
                table: "REOPENING_LOGS");

            migrationBuilder.RenameColumn(
                name: "TITLE",
                table: "TICKETS",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "STATUS",
                table: "TICKETS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "PRIORITY",
                table: "TICKETS",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "DESCRIPTION",
                table: "TICKETS",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "TICKETS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UPDATED_AT",
                table: "TICKETS",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "SLA_MINUTES",
                table: "TICKETS",
                newName: "SlaMinutes");

            migrationBuilder.RenameColumn(
                name: "CREATED_BY_WINDOWS_USER",
                table: "TICKETS",
                newName: "CreatedByWindowsUser");

            migrationBuilder.RenameColumn(
                name: "CREATED_BY_HOSTNAME",
                table: "TICKETS",
                newName: "CreatedByHostname");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "TICKETS",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "TITLE",
                table: "TICKET_LOGS",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "STATUS",
                table: "TICKET_LOGS",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "PRIORITY",
                table: "TICKET_LOGS",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "DESCRIPTION",
                table: "TICKET_LOGS",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "TICKET_LOGS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UPDATED_AT",
                table: "TICKET_LOGS",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "SLA_MINUTES",
                table: "TICKET_LOGS",
                newName: "SlaMinutes");

            migrationBuilder.RenameColumn(
                name: "ORIGINAL_TICKET_ID",
                table: "TICKET_LOGS",
                newName: "OriginalTicketId");

            migrationBuilder.RenameColumn(
                name: "DELETION_TIMESTAMP",
                table: "TICKET_LOGS",
                newName: "DeletionTimestamp");

            migrationBuilder.RenameColumn(
                name: "DELETED_BY_WINDOWS_USER",
                table: "TICKET_LOGS",
                newName: "DeletedByWindowsUser");

            migrationBuilder.RenameColumn(
                name: "DELETED_BY_HOSTNAME",
                table: "TICKET_LOGS",
                newName: "DeletedByHostname");

            migrationBuilder.RenameColumn(
                name: "CREATED_AT",
                table: "TICKET_LOGS",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "REASON",
                table: "REOPENING_LOGS",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "REOPENING_LOGS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TICKET_ID",
                table: "REOPENING_LOGS",
                newName: "TicketId");

            migrationBuilder.RenameColumn(
                name: "REOPENED_BY",
                table: "REOPENING_LOGS",
                newName: "ReopenedBy");

            migrationBuilder.RenameColumn(
                name: "REOPENED_AT",
                table: "REOPENING_LOGS",
                newName: "ReopenedAt");

            migrationBuilder.RenameIndex(
                name: "IX_REOPENING_LOGS_TICKET_ID",
                table: "REOPENING_LOGS",
                newName: "IX_REOPENING_LOGS_TicketId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "AUTHORIZED_USERS",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WINDOWS_USER_NAME",
                table: "AUTHORIZED_USERS",
                newName: "WindowsUserName");

            migrationBuilder.RenameColumn(
                name: "IS_ADMIN",
                table: "AUTHORIZED_USERS",
                newName: "IsAdmin");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TICKETS",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldComment: "0=Aberto, 1=EmAndamento, 2=Resolvido, 3=Fechado");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "TICKETS",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldComment: "0=Baixa, 1=Media, 2=Alta");

            migrationBuilder.AddForeignKey(
                name: "FK_REOPENING_LOGS_TICKETS_TicketId",
                table: "REOPENING_LOGS",
                column: "TicketId",
                principalTable: "TICKETS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
