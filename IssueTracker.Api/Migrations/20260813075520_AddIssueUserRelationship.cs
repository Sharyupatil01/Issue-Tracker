using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Issues");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AssignedToUserId",
                table: "Issues",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Users_AssignedToUserId",
                table: "Issues",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Users_AssignedToUserId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_AssignedToUserId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Issues");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Issues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
