using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineBenavides.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservaItems_reservas_ReservaId",
                table: "reservaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_reservas_funciones_FuncionId",
                table: "reservas");

            migrationBuilder.AddForeignKey(
                name: "FK_reservaItems_reservas_ReservaId",
                table: "reservaItems",
                column: "ReservaId",
                principalTable: "reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservas_funciones_FuncionId",
                table: "reservas",
                column: "FuncionId",
                principalTable: "funciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservaItems_reservas_ReservaId",
                table: "reservaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_reservas_funciones_FuncionId",
                table: "reservas");

            migrationBuilder.AddForeignKey(
                name: "FK_reservaItems_reservas_ReservaId",
                table: "reservaItems",
                column: "ReservaId",
                principalTable: "reservas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reservas_funciones_FuncionId",
                table: "reservas",
                column: "FuncionId",
                principalTable: "funciones",
                principalColumn: "Id");
        }
    }
}
