using Microsoft.EntityFrameworkCore.Migrations;

namespace code_hunter.Migrations
{
    public partial class UsefulQACount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsefulAnswersCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsefulQuestionsCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsefulAnswersCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UsefulQuestionsCount",
                table: "AspNetUsers");
        }
    }
}
