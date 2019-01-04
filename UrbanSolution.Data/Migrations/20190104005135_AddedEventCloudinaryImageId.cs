using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class AddedEventCloudinaryImageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Events",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                maxLength: 3000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<int>(
                name: "CloudinaryImageId",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CloudinaryImageId",
                table: "Events",
                column: "CloudinaryImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_CloudinaryImages_CloudinaryImageId",
                table: "Events",
                column: "CloudinaryImageId",
                principalTable: "CloudinaryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_CloudinaryImages_CloudinaryImageId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CloudinaryImageId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CloudinaryImageId",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Events",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 70);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 3000);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Events",
                nullable: true);
        }
    }
}
