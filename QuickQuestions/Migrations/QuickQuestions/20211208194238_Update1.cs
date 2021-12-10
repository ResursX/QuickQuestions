using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickQuestions.Migrations.QuickQuestions
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerLayout",
                table: "Question");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnswerLayout",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
