using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class MeargedEveryWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedColorsRaw",
                table: "products");

            migrationBuilder.DropColumn(
                name: "SelectedSizesRaw",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "order_items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "order_items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "cart_items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "size",
                table: "cart_items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "saved_carts",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saved_carts", x => x.id);
                    table.ForeignKey(
                        name: "FK_saved_carts_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_cart_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    saved_cart_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    product_id = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    added_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saved_cart_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_saved_cart_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_saved_cart_items_saved_carts_saved_cart_id",
                        column: x => x.saved_cart_id,
                        principalTable: "saved_carts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_saved_cart_items_product_id",
                table: "saved_cart_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_saved_cart_items_saved_cart_id",
                table: "saved_cart_items",
                column: "saved_cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_saved_carts_user_id",
                table: "saved_carts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saved_cart_items");

            migrationBuilder.DropTable(
                name: "saved_carts");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "color",
                table: "cart_items");

            migrationBuilder.DropColumn(
                name: "size",
                table: "cart_items");

            migrationBuilder.AddColumn<string>(
                name: "SelectedColorsRaw",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedSizesRaw",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
