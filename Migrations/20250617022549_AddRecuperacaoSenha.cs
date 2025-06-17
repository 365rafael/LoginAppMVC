using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginAppMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddRecuperacaoSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiracao",
                table: "Usuario",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenRecuperacao",
                table: "Usuario",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenExpiracao",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenRecuperacao",
                table: "Usuario");
        }
    }
}
