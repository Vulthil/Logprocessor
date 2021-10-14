using Microsoft.EntityFrameworkCore.Migrations;

namespace LogProcessor.Migrations
{
    public partial class RenameInternalToExternal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InternalServices",
                table: "SessionTypes",
                newName: "ExternalServices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalServices",
                table: "SessionTypes",
                newName: "InternalServices");
        }
    }
}
