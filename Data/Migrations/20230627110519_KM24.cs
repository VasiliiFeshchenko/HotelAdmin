using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class KM24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Order_OrderSource_SourceId",
            //    table: "Order");

            //migrationBuilder.DropIndex(
            //    name: "IX_Order_SourceId",
            //    table: "Order");

            //migrationBuilder.DropColumn(
            //    name: "SourceId",
            //    table: "Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_SourceId",
                table: "Order",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderSource_SourceId",
                table: "Order",
                column: "SourceId",
                principalTable: "OrderSource",
                principalColumn: "Id");
        }
    }
}
