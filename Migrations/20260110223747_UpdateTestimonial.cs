using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neksara.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestimonial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Testimonis",
                newName: "TestimoniRole");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Testimonis",
                newName: "TestimoniRating");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Testimonis",
                newName: "TestimoniName");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Testimonis",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "TestimoniRole",
                table: "Testimonis",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "TestimoniRating",
                table: "Testimonis",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "TestimoniName",
                table: "Testimonis",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Testimonis",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
