using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixstudentassignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssignment_Assignment_AssignmentId",
                table: "StudentAssignment");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignmentId",
                table: "StudentAssignment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignment_Assignment_AssignmentId",
                table: "StudentAssignment",
                column: "AssignmentId",
                principalTable: "Assignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssignment_Assignment_AssignmentId",
                table: "StudentAssignment");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignmentId",
                table: "StudentAssignment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignment_Assignment_AssignmentId",
                table: "StudentAssignment",
                column: "AssignmentId",
                principalTable: "Assignment",
                principalColumn: "Id");
        }
    }
}
