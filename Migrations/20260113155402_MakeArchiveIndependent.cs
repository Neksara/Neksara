using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neksara.Migrations
{
    /// <inheritdoc />
    public partial class MakeArchiveIndependent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchiveTopics_Category_CategoryId",
                table: "ArchiveTopics");

            migrationBuilder.DropIndex(
                name: "IX_ArchiveTopics_CategoryId",
                table: "ArchiveTopics");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ArchiveTopics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "ArchiveTopics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "ArchiveTopics");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ArchiveTopics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveTopics_CategoryId",
                table: "ArchiveTopics",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchiveTopics_Category_CategoryId",
                table: "ArchiveTopics",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
