using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class KM8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPricePerNight",
                table: "BookableObject");

            migrationBuilder.AlterColumn<decimal>(
                name: "IsDogAllowed",
                table: "BookableObject",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDogAllowed",
                table: "BookableObject",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPricePerNight",
                table: "BookableObject",
                type: "decimal(10,2)",
                nullable: true);
        }
    }
}
