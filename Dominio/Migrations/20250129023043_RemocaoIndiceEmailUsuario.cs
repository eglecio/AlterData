using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dominio.Migrations {
  /// <inheritdoc />
  public partial class RemocaoIndiceEmailUsuario : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropIndex("IX_Clientes_CPF");
      migrationBuilder.DropIndex("IX_Usuarios_Email");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateIndex(
        name: "IX_Clientes_CPF",
        table: "Clientes",
        column: "CPF",
        unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Usuarios_Email",
          table: "Usuarios",
          column: "Email",
          unique: true);
    }
  }
}
