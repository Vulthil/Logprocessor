using Microsoft.EntityFrameworkCore.Migrations;

namespace LogProcessor.Migrations
{
    public partial class RemoveFileAddTextColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "SessionTypes",
                newName: "Text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "SessionTypes",
                newName: "FileName");
        }
    }
}
