using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class addOrderRevertedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders_Reverted",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    orderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    order_itemId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RevertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_Reverted", x => x.id);
                    table.ForeignKey(
                        name: "FK_Orders_Reverted_order_items_order_itemId",
                        column: x => x.order_itemId,
                        principalTable: "order_items",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Orders_Reverted_orders_orderId",
                        column: x => x.orderId,
                        principalTable: "orders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Reverted_order_itemId",
                table: "Orders_Reverted",
                column: "order_itemId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Reverted_orderId",
                table: "Orders_Reverted",
                column: "orderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders_Reverted");
        }
    }
}
