using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawBuddy.Data.Migrations
{
    /// <inheritdoc />
    public partial class novacomcd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodPostal",
                table: "Utilizador",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodPostal",
                table: "Utilizador");
        }
    }
}
