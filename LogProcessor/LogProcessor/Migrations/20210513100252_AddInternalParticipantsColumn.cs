using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogProcessor.Migrations
{
    public partial class AddInternalParticipantsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalServices",
                table: "SessionTypes",
                newName: "InternalParticipants");

            migrationBuilder.AddColumn<string[]>(
                name: "ExternalParticipants",
                table: "SessionTypes",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalParticipants",
                table: "SessionTypes");

            migrationBuilder.RenameColumn(
                name: "InternalParticipants",
                table: "SessionTypes",
                newName: "ExternalServices");
        }
    }
}
