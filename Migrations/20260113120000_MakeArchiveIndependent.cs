using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neksara.Migrations
{
    public partial class MakeArchiveIndependent_Old : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing FK and index from ArchiveTopics -> Category
            migrationBuilder.DropForeignKey(
                name: "FK_ArchiveTopics_Category_CategoryId",
                table: "ArchiveTopics");

            migrationBuilder.DropIndex(
                name: "IX_ArchiveTopics_CategoryId",
                table: "ArchiveTopics");

            // Make CategoryId nullable
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ArchiveTopics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Add CategoryName snapshot column to ArchiveTopics
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "ArchiveTopics",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            // Create ArchiveCategories table
            migrationBuilder.CreateTable(
                name: "ArchiveCategories",
                columns: table => new
                {
                    ArchiveCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalCategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryPicture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveCategories", x => x.ArchiveCategoryId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop ArchiveCategories
            migrationBuilder.DropTable(
                name: "ArchiveCategories");

            // Remove CategoryName from ArchiveTopics
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "ArchiveTopics");

            // Make CategoryId non-nullable again
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ArchiveTopics",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Recreate index and FK
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
