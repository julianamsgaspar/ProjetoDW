using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawBuddy.Data.Migrations
{
    /// <inheritdoc />
    public partial class alteracoesregisterlogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserName",
                table: "Utilizador",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserName",
                table: "Utilizador");
        }
    }
}
