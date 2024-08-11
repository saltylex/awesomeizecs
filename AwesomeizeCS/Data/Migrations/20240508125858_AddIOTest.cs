using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIOTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IOTest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOTest_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ProvidedInput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IOTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestStep_IOTest_IOTestId",
                        column: x => x.IOTestId,
                        principalTable: "IOTest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IOTest_AssignmentId",
                table: "IOTest",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStep_IOTestId",
                table: "TestStep",
                column: "IOTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestStep");

            migrationBuilder.DropTable(
                name: "IOTest");
        }
    }
}
