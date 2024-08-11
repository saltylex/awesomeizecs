using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addponderedintotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PercentageOutOfTotal",
                table: "Assignment",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentageOutOfTotal",
                table: "Assignment");
        }
    }
}
