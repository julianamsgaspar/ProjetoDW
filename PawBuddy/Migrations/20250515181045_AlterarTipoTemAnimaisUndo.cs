using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawBuddy.Migrations
{
    /// <inheritdoc />
    public partial class AlterarTipoTemAnimaisUndo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "temAnimais",
                table: "Intencao",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "temAnimais",
                table: "Intencao",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
