using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Fridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ingredients",
                table: "FridgeItems",
                newName: "Ingredient");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "FridgeItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "FridgeItems");

            migrationBuilder.RenameColumn(
                name: "Ingredient",
                table: "FridgeItems",
                newName: "Ingredients");
        }
    }
}
