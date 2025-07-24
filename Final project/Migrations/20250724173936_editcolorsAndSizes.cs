using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class editcolorsAndSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SelectedSizes",
                table: "products",
                newName: "SelectedSizesRaw");

            migrationBuilder.RenameColumn(
                name: "SelectedColors",
                table: "products",
                newName: "SelectedColorsRaw");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SelectedSizesRaw",
                table: "products",
                newName: "SelectedSizes");

            migrationBuilder.RenameColumn(
                name: "SelectedColorsRaw",
                table: "products",
                newName: "SelectedColors");
        }
    }
}
