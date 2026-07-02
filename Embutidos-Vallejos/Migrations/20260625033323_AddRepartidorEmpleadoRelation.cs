using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Embutidos_Vallejos.Migrations
{
    /// <inheritdoc />
    public partial class AddRepartidorEmpleadoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpleadoId",
                table: "Repartidor",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repartidor_EmpleadoId",
                table: "Repartidor",
                column: "EmpleadoId",
                unique: true,
                filter: "[EmpleadoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Repartidor_Empleado_EmpleadoId",
                table: "Repartidor",
                column: "EmpleadoId",
                principalTable: "Empleado",
                principalColumn: "EmpleadoId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repartidor_Empleado_EmpleadoId",
                table: "Repartidor");

            migrationBuilder.DropIndex(
                name: "IX_Repartidor_EmpleadoId",
                table: "Repartidor");

            migrationBuilder.DropColumn(
                name: "EmpleadoId",
                table: "Repartidor");
        }
    }
}
