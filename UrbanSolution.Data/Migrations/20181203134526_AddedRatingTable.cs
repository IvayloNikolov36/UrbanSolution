using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class AddedRatingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UrbanIssues_ResolvedIssues_ResolvedIssueId",
                table: "UrbanIssues");

            migrationBuilder.DropIndex(
                name: "IX_UrbanIssues_ResolvedIssueId",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "ResolvedIssueId",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "Evaluation",
                table: "ResolvedIssues");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "ResolvedIssues",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "UrbanIssueId",
                table: "ResolvedIssues",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    ResolvedIssueId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_ResolvedIssues_ResolvedIssueId",
                        column: x => x.ResolvedIssueId,
                        principalTable: "ResolvedIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResolvedIssues_UrbanIssueId",
                table: "ResolvedIssues",
                column: "UrbanIssueId",
                unique: true,
                filter: "[UrbanIssueId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ResolvedIssueId",
                table: "Ratings",
                column: "ResolvedIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResolvedIssues_UrbanIssues_UrbanIssueId",
                table: "ResolvedIssues",
                column: "UrbanIssueId",
                principalTable: "UrbanIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResolvedIssues_UrbanIssues_UrbanIssueId",
                table: "ResolvedIssues");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_ResolvedIssues_UrbanIssueId",
                table: "ResolvedIssues");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ResolvedIssues");

            migrationBuilder.DropColumn(
                name: "UrbanIssueId",
                table: "ResolvedIssues");

            migrationBuilder.AddColumn<int>(
                name: "ResolvedIssueId",
                table: "UrbanIssues",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Evaluation",
                table: "ResolvedIssues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrbanIssues_ResolvedIssueId",
                table: "UrbanIssues",
                column: "ResolvedIssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_UrbanIssues_ResolvedIssues_ResolvedIssueId",
                table: "UrbanIssues",
                column: "ResolvedIssueId",
                principalTable: "ResolvedIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
