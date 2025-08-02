using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class mmmmmmmmmmmmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSellerActevated",
                table: "products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSellerActevated",
                table: "products");
        }
    }
}
