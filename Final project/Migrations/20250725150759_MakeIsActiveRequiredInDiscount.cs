using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class MakeIsActiveRequiredInDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_orderid",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_product_images_products_productid",
                table: "product_images");

            migrationBuilder.DropIndex(
                name: "IX_product_images_productid",
                table: "product_images");

            migrationBuilder.DropIndex(
                name: "IX_order_items_orderid",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "productid",
                table: "product_images");

            migrationBuilder.DropColumn(
                name: "orderid",
                table: "order_items");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "discounts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "productid",
                table: "product_images",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "orderid",
                table: "order_items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "discounts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_productid",
                table: "product_images",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_orderid",
                table: "order_items",
                column: "orderid");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_orderid",
                table: "order_items",
                column: "orderid",
                principalTable: "orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_images_products_productid",
                table: "product_images",
                column: "productid",
                principalTable: "products",
                principalColumn: "id");
        }
    }
}
