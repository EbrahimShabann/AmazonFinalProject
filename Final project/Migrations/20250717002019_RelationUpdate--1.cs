using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class RelationUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "orders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "order_items",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "order_items",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_history_ticket_id",
                table: "ticket_history",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_product_id",
                table: "product_images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_discounts_discount_id",
                table: "product_discounts",
                column: "discount_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_discounts_product_id",
                table: "product_discounts",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_history_order_id",
                table: "order_history",
                column: "order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_history_orders_order_id",
                table: "order_history",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_products_product_id",
                table: "order_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_discounts_discounts_discount_id",
                table: "product_discounts",
                column: "discount_id",
                principalTable: "discounts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_discounts_products_product_id",
                table: "product_discounts",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_images_products_product_id",
                table: "product_images",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_history_support_tickets_ticket_id",
                table: "ticket_history",
                column: "ticket_id",
                principalTable: "support_tickets",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_history_orders_order_id",
                table: "order_history");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_products_product_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_product_discounts_discounts_discount_id",
                table: "product_discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_product_discounts_products_product_id",
                table: "product_discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_product_images_products_product_id",
                table: "product_images");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_history_support_tickets_ticket_id",
                table: "ticket_history");

            migrationBuilder.DropIndex(
                name: "IX_ticket_history_ticket_id",
                table: "ticket_history");

            migrationBuilder.DropIndex(
                name: "IX_product_images_product_id",
                table: "product_images");

            migrationBuilder.DropIndex(
                name: "IX_product_discounts_discount_id",
                table: "product_discounts");

            migrationBuilder.DropIndex(
                name: "IX_product_discounts_product_id",
                table: "product_discounts");

            migrationBuilder.DropIndex(
                name: "IX_order_items_order_id",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_order_items_product_id",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_order_history_order_id",
                table: "order_history");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "order_items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "order_items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
