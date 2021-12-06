using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tokens.Infrastructure.Persistence.Database.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", nullable: false),
                    CreatedBy = table.Column<string>(type: "char(36)", nullable: false),
                    CreatedByDevice = table.Column<string>(type: "char(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", nullable: false),
                    TokenId = table.Column<string>(type: "char(20)", nullable: false),
                    Accessor = table.Column<string>(type: "char(36)", nullable: false),
                    AccessedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessLog_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLog_TokenId",
                table: "AccessLog",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_CreatedBy",
                table: "Tokens",
                column: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLog");

            migrationBuilder.DropTable(
                name: "Tokens");
        }
    }
}
