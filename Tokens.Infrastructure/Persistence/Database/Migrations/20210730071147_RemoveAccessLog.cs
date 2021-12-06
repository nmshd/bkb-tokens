using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tokens.Infrastructure.Persistence.Database.Migrations
{
    public partial class RemoveAccessLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(20)", nullable: false),
                    AccessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accessor = table.Column<string>(type: "char(36)", nullable: false),
                    TokenId = table.Column<string>(type: "char(20)", nullable: false)
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
        }
    }
}
