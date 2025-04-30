using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawBuddy.Data.Migrations
{
    /// <inheritdoc />
    public partial class adicionousestringlength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Intencao",
                table: "Intencao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doa",
                table: "Doa");

            migrationBuilder.AlterColumn<string>(
                name: "Residencia",
                table: "Intencao",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Profissao",
                table: "Intencao",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Intencao",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Contacto",
                table: "Intencao",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Intencao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Doa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Raca",
                table: "Animal",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Animal",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Idade",
                table: "Animal",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "Animal",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Especie",
                table: "Animal",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cor",
                table: "Animal",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Adotam",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Intencao",
                table: "Intencao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doa",
                table: "Doa",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Intencao_UtilizadorFK",
                table: "Intencao",
                column: "UtilizadorFK");

            migrationBuilder.CreateIndex(
                name: "IX_Doa_UtilizadorFK",
                table: "Doa",
                column: "UtilizadorFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Intencao",
                table: "Intencao");

            migrationBuilder.DropIndex(
                name: "IX_Intencao_UtilizadorFK",
                table: "Intencao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doa",
                table: "Doa");

            migrationBuilder.DropIndex(
                name: "IX_Doa_UtilizadorFK",
                table: "Doa");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Intencao");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Doa");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Adotam");

            migrationBuilder.AlterColumn<string>(
                name: "Residencia",
                table: "Intencao",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Profissao",
                table: "Intencao",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "Intencao",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Contacto",
                table: "Intencao",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Raca",
                table: "Animal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Animal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Idade",
                table: "Animal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "Animal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Especie",
                table: "Animal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Cor",
                table: "Animal",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Intencao",
                table: "Intencao",
                columns: new[] { "UtilizadorFK", "AnimalFK" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doa",
                table: "Doa",
                columns: new[] { "UtilizadorFK", "AnimalFK" });
        }
    }
}
