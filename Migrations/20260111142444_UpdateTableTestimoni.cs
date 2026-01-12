using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neksara.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableTestimoni : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Category");
        }
    }
}
