using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogProcessor.Migrations
{
    public partial class AddInternalServicesColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "InternalServices",
                table: "SessionTypes",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalServices",
                table: "SessionTypes");
        }
    }
}
