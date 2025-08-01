using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedCartSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "saved_carts",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "saved_cart_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    saved_cart_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    product_id = table.Column<string>(type: "nvarchar(255)", nullable: true),
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
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_saved_cart_items_saved_carts_saved_cart_id",
                        column: x => x.saved_cart_id,
                        principalTable: "saved_carts",
                        principalColumn: "id");
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
        }
    }
}
