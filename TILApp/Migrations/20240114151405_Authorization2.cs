using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TILApp.Migrations
{
    /// <inheritdoc />
    public partial class Authorization2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acronym_AspNetUsers_UserId1",
                table: "Acronym");

            migrationBuilder.DropIndex(
                name: "IX_Acronym_UserId1",
                table: "Acronym");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Acronym");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acronym_UserId",
                table: "Acronym",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Acronym_User_UserId",
                table: "Acronym",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acronym_User_UserId",
                table: "Acronym");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Acronym_UserId",
                table: "Acronym");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Acronym",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Acronym_UserId1",
                table: "Acronym",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Acronym_AspNetUsers_UserId1",
                table: "Acronym",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
