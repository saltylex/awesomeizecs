using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeizeCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixassignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Assignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_ParentId",
                table: "Assignment",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Assignment_ParentId",
                table: "Assignment",
                column: "ParentId",
                principalTable: "Assignment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Assignment_ParentId",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_ParentId",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Assignment");
        }
    }
}
