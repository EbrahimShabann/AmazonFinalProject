using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class addListSizesAndListColorsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colors",
                table: "products");

            migrationBuilder.DropColumn(
                name: "Sizes",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "SelectedColors",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedSizes",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedColors",
                table: "products");

            migrationBuilder.DropColumn(
                name: "SelectedSizes",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "Colors",
                table: "products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sizes",
                table: "products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
