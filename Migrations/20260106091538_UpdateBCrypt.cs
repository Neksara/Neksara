using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neksara.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBCrypt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TopicView",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryPicture",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TopicView_CategoryId",
                table: "TopicView",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicView_Category_CategoryId",
                table: "TopicView",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopicView_Category_CategoryId",
                table: "TopicView");

            migrationBuilder.DropIndex(
                name: "IX_TopicView_CategoryId",
                table: "TopicView");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TopicView");

            migrationBuilder.DropColumn(
                name: "CategoryPicture",
                table: "Category");
        }
    }
}
