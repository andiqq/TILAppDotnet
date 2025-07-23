using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TILApp.Migrations
{
    /// <inheritdoc />
    public partial class EnforceParentChildContraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acronym_AspNetUsers_UserId",
                table: "Acronym");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Acronym",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Acronym_AspNetUsers_UserId",
                table: "Acronym",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acronym_AspNetUsers_UserId",
                table: "Acronym");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Acronym",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Acronym_AspNetUsers_UserId",
                table: "Acronym",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
