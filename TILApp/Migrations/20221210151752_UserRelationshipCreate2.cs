using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TILApp.Migrations
{
    /// <inheritdoc />
    public partial class UserRelationshipCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acronym_User_UserId",
                table: "Acronym");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Acronym",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Acronym_User_UserId",
                table: "Acronym",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acronym_User_UserId",
                table: "Acronym");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Acronym",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Acronym_User_UserId",
                table: "Acronym",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
