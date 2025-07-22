using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_project.Migrations
{
    /// <inheritdoc />
    public partial class sellerIdInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
