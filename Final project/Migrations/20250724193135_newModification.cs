using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class newModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_Users_seller_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_seller_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "seller_id",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "image_type",
                table: "product_images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "seller_id",
                table: "order_items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "order_items",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "discounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "chat_sessionId",
                table: "chat_messages",
                type: "nvarchar(255)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "review_reply",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    review_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    replier_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    reply_text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_seller_reply = table.Column<bool>(type: "bit", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_reply", x => x.id);
                    table.ForeignKey(
                        name: "FK_review_reply_Users_replier_id",
                        column: x => x.replier_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_review_reply_product_reviews_review_id",
                        column: x => x.review_id,
                        principalTable: "product_reviews",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_product_id",
                table: "product_reviews",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_seller_id",
                table: "order_items",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_chat_sessionId",
                table: "chat_messages",
                column: "chat_sessionId");

            migrationBuilder.CreateIndex(
                name: "IX_review_reply_replier_id",
                table: "review_reply",
                column: "replier_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_reply_review_id",
                table: "review_reply",
                column: "review_id");

            migrationBuilder.AddForeignKey(
                name: "FK_chat_messages_chat_sessions_chat_sessionId",
                table: "chat_messages",
                column: "chat_sessionId",
                principalTable: "chat_sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_Users_seller_id",
                table: "order_items",
                column: "seller_id",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_reviews_products_product_id",
                table: "product_reviews",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_messages_chat_sessions_chat_sessionId",
                table: "chat_messages");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_Users_seller_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_product_reviews_products_product_id",
                table: "product_reviews");

            migrationBuilder.DropTable(
                name: "review_reply");

            migrationBuilder.DropIndex(
                name: "IX_product_reviews_product_id",
                table: "product_reviews");

            migrationBuilder.DropIndex(
                name: "IX_order_items_seller_id",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_chat_messages_chat_sessionId",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "image_type",
                table: "product_images");

            migrationBuilder.DropColumn(
                name: "seller_id",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "status",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "discounts");

            migrationBuilder.DropColumn(
                name: "chat_sessionId",
                table: "chat_messages");

            migrationBuilder.AddColumn<string>(
                name: "seller_id",
                table: "orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_seller_id",
                table: "orders",
                column: "seller_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_Users_seller_id",
                table: "orders",
                column: "seller_id",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
