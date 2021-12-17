using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickQuestions.Migrations.QuickQuestions
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "File",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "File");
        }
    }
}
