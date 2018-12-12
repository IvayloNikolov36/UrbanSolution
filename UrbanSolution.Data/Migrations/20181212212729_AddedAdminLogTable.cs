using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class AddedAdminLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdminId = table.Column<string>(nullable: true),
                    EditedUserId = table.Column<string>(nullable: true),
                    Activity = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminLogs_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminLogs_AspNetUsers_EditedUserId",
                        column: x => x.EditedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminLogs_AdminId",
                table: "AdminLogs",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminLogs_EditedUserId",
                table: "AdminLogs",
                column: "EditedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminLogs");
        }
    }
}
