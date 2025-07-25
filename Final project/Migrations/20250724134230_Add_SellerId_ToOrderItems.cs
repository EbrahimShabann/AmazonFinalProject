using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class Add_SellerId_ToOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_Users_user_id",
                table: "audit_logs");

            

            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropTable(
                name: "chat_sessions");

           

            migrationBuilder.DropIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK__AccountLog__5E5499A8A33FBCA4",
                table: "AccountLog");

            

            migrationBuilder.DropColumn(
                name: "entity_id",
                table: "audit_logs");

            migrationBuilder.RenameColumn(
                name: "entity_type",
                table: "audit_logs",
                newName: "table_name");

            

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

            migrationBuilder.AddColumn<string>(
                name: "seller_id",
                table: "order_items",
                type: "nvarchar(450)",
                nullable: true);

            

            

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "audit_logs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "audit_logs",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "record_id",
                table: "audit_logs",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "AccountLog",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountLog",
                table: "AccountLog",
                column: "LogID");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_messages_ticket_id",
                table: "ticket_messages",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_productid",
                table: "product_images",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_orderid",
                table: "order_items",
                column: "orderid");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_seller_id",
                table: "order_items",
                column: "seller_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_Users_seller_id",
                table: "order_items",
                column: "seller_id",
                principalTable: "Users",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ticket_messages_support_tickets_ticket_id",
                table: "ticket_messages",
                column: "ticket_id",
                principalTable: "support_tickets",
                principalColumn: "id");

            migrationBuilder.AlterColumn<string>(
                name: "category_id",
                table: "products",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "categories",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_Users_seller_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_orderid",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_product_images_products_productid",
                table: "product_images");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_category_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_ticket_messages_support_tickets_ticket_id",
                table: "ticket_messages");

            migrationBuilder.DropIndex(
                name: "IX_ticket_messages_ticket_id",
                table: "ticket_messages");

            migrationBuilder.DropIndex(
                name: "IX_products_category_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_product_images_productid",
                table: "product_images");

            migrationBuilder.DropIndex(
                name: "IX_order_items_orderid",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_order_items_seller_id",
                table: "order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountLog",
                table: "AccountLog");

            migrationBuilder.DropColumn(
                name: "image_type",
                table: "product_images");

            migrationBuilder.DropColumn(
                name: "productid",
                table: "product_images");

            migrationBuilder.DropColumn(
                name: "orderid",
                table: "order_items");

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
                name: "record_id",
                table: "audit_logs");

            migrationBuilder.RenameColumn(
                name: "table_name",
                table: "audit_logs",
                newName: "entity_type");

            migrationBuilder.AddColumn<string>(
                name: "seller_id",
                table: "orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "audit_logs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "audit_logs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "entity_id",
                table: "audit_logs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStamp",
                table: "AccountLog",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__AccountLog__5E5499A8A33FBCA4",
                table: "AccountLog",
                column: "LogID");

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sender_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sent_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    session_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_chat_messages_Users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "chat_sessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SellerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_sessions_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chat_sessions_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_sender_id",
                table: "chat_messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_sessions_CustomerId",
                table: "chat_sessions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_sessions_SellerId",
                table: "chat_sessions",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_user_id",
                table: "product_reviews",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_Users_user_id",
                table: "audit_logs",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id");

            
        }
    }
}
