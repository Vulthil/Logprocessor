using Microsoft.EntityFrameworkCore.Migrations;

namespace LogProcessor.Migrations
{
    public partial class SessionIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LogMessages_Message_SessionId",
                table: "LogMessages",
                column: "Message_SessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogMessages_Message_SessionId",
                table: "LogMessages");
        }
    }
}
