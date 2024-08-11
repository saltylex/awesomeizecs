using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifytc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherName",
                table: "TeacherCourse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherName",
                table: "TeacherCourse");
        }
    }
}
