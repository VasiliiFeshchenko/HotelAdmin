using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class KM6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOwnKitchen",
                table: "Category",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AdditionalPrice",
                table: "BookableObject",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDogAllowed",
                table: "BookableObject",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAdditionalBeds",
                table: "BookableObject",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOwnKitchen",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "AdditionalPrice",
                table: "BookableObject");

            migrationBuilder.DropColumn(
                name: "IsDogAllowed",
                table: "BookableObject");

            migrationBuilder.DropColumn(
                name: "NumberOfAdditionalBeds",
                table: "BookableObject");
        }
    }
}
