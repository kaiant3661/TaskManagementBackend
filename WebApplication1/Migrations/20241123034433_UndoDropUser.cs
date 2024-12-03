using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UndoDropUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Userr_PerformedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Userr_Roles_RoleId",
                table: "Userr");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Userr_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId",
                principalTable: "Userr",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Userr_Roles_RoleId",
                table: "Userr",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Userr_PerformedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Userr_Roles_RoleId",
                table: "Userr");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Userr_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId",
                principalTable: "Userr",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Userr_Roles_RoleId",
                table: "Userr",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
