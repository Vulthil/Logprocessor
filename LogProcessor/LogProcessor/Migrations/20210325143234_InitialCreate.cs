using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LogProcessor.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Message_Origin = table.Column<string>(type: "text", nullable: true),
                    Message_Destination = table.Column<string>(type: "text", nullable: true),
                    Message_TargetApi = table.Column<string>(type: "text", nullable: true),
                    Message_SessionId = table.Column<string>(type: "text", nullable: true),
                    Message_Direction = table.Column<int>(type: "integer", nullable: true),
                    Message_Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViolatingMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolatingMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolatingMessages_LogMessages_LogMessageId",
                        column: x => x.LogMessageId,
                        principalTable: "LogMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoisonedMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogMessageId = table.Column<int>(type: "integer", nullable: false),
                    ViolatingMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoisonedMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoisonedMessages_LogMessages_LogMessageId",
                        column: x => x.LogMessageId,
                        principalTable: "LogMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoisonedMessages_ViolatingMessages_ViolatingMessageId",
                        column: x => x.ViolatingMessageId,
                        principalTable: "ViolatingMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoisonedMessages_LogMessageId",
                table: "PoisonedMessages",
                column: "LogMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_PoisonedMessages_ViolatingMessageId",
                table: "PoisonedMessages",
                column: "ViolatingMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolatingMessages_LogMessageId",
                table: "ViolatingMessages",
                column: "LogMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoisonedMessages");

            migrationBuilder.DropTable(
                name: "ViolatingMessages");

            migrationBuilder.DropTable(
                name: "LogMessages");
        }
    }
}
