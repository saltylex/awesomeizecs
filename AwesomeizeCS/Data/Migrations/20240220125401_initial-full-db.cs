using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialfulldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToughtBy",
                table: "TimeTable",
                newName: "TaughtBy");

            migrationBuilder.RenameColumn(
                name: "HasParrent",
                table: "Assignment",
                newName: "HasParent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaughtBy",
                table: "TimeTable",
                newName: "ToughtBy");

            migrationBuilder.RenameColumn(
                name: "HasParent",
                table: "Assignment",
                newName: "HasParrent");
        }
    }
}
