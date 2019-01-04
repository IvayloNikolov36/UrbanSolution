using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class AddedCloudinaryImageColumnToArticlesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrbanServices");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "CloudinaryImageId",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CloudinaryImageId",
                table: "Articles",
                column: "CloudinaryImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_CloudinaryImages_CloudinaryImageId",
                table: "Articles",
                column: "CloudinaryImageId",
                principalTable: "CloudinaryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_CloudinaryImages_CloudinaryImageId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CloudinaryImageId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CloudinaryImageId",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Articles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UrbanServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    OfferedSince = table.Column<DateTime>(nullable: false),
                    PictureUrl = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrbanServices", x => x.Id);
                });
        }
    }
}
