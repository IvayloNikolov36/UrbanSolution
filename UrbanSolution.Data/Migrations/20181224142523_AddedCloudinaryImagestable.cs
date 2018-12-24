using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class AddedCloudinaryImagestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuePictureUrl",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "ResolvedIssues");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UrbanIssues",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "CloudinaryImageId",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CloudinaryImageId",
                table: "ResolvedIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CloudinaryImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PicturePublicId = table.Column<string>(nullable: true),
                    PictureUrl = table.Column<string>(nullable: true),
                    PictureThumbnailUrl = table.Column<string>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    UploadedOn = table.Column<DateTime>(nullable: false),
                    UploaderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudinaryImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudinaryImages_AspNetUsers_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrbanIssues_CloudinaryImageId",
                table: "UrbanIssues",
                column: "CloudinaryImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ResolvedIssues_CloudinaryImageId",
                table: "ResolvedIssues",
                column: "CloudinaryImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudinaryImages_UploaderId",
                table: "CloudinaryImages",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResolvedIssues_CloudinaryImages_CloudinaryImageId",
                table: "ResolvedIssues",
                column: "CloudinaryImageId",
                principalTable: "CloudinaryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UrbanIssues_CloudinaryImages_CloudinaryImageId",
                table: "UrbanIssues",
                column: "CloudinaryImageId",
                principalTable: "CloudinaryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResolvedIssues_CloudinaryImages_CloudinaryImageId",
                table: "ResolvedIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_UrbanIssues_CloudinaryImages_CloudinaryImageId",
                table: "UrbanIssues");

            migrationBuilder.DropTable(
                name: "CloudinaryImages");

            migrationBuilder.DropIndex(
                name: "IX_UrbanIssues_CloudinaryImageId",
                table: "UrbanIssues");

            migrationBuilder.DropIndex(
                name: "IX_ResolvedIssues_CloudinaryImageId",
                table: "ResolvedIssues");

            migrationBuilder.DropColumn(
                name: "CloudinaryImageId",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "CloudinaryImageId",
                table: "ResolvedIssues");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "UrbanIssues",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "IssuePictureUrl",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "ResolvedIssues",
                nullable: false,
                defaultValue: "");
        }
    }
}
