using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedcodeversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeForId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeVersion_StudentAssignment_CodeForId",
                        column: x => x.CodeForId,
                        principalTable: "StudentAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResult_CodeVersion_CodeVersionId",
                        column: x => x.CodeVersionId,
                        principalTable: "CodeVersion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestResult_IOTest_TestId",
                        column: x => x.TestId,
                        principalTable: "IOTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeVersion_CodeForId",
                table: "CodeVersion",
                column: "CodeForId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_CodeVersionId",
                table: "TestResult",
                column: "CodeVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_TestId",
                table: "TestResult",
                column: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResult");

            migrationBuilder.DropTable(
                name: "CodeVersion");
        }
    }
}
