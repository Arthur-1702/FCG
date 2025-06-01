using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class start : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Level = table.Column<string>(type: "NVARCHAR(128)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Exception = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Properties = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    NivelAcesso = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Saldo = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false),
                    Preco = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Genero = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UsuarioId = table.Column<int>(type: "INT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogo_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Promocao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "VARCHAR(100)", maxLength: 50, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DataFim = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Ativo = table.Column<bool>(type: "BIT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promocao_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JogosPromocoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JogoId = table.Column<int>(type: "INT", nullable: false),
                    PromocaoId = table.Column<int>(type: "INT", nullable: false),
                    Desconto = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    UsuarioId = table.Column<int>(type: "INT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogosPromocoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JogosPromocoes_Jogo_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JogosPromocoes_Promocao_PromocaoId",
                        column: x => x.PromocaoId,
                        principalTable: "Promocao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JogosPromocoes_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UsuarioJogoPropriedade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "INT", nullable: false),
                    JogoId = table.Column<int>(type: "INT", nullable: false),
                    ValorPago = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    PromocaoId = table.Column<int>(type: "INT", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioJogoPropriedade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioJogoPropriedade_Jogo_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioJogoPropriedade_Promocao_PromocaoId",
                        column: x => x.PromocaoId,
                        principalTable: "Promocao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UsuarioJogoPropriedade_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jogo_UsuarioId",
                table: "Jogo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_JogosPromocoes_JogoId",
                table: "JogosPromocoes",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_JogosPromocoes_PromocaoId",
                table: "JogosPromocoes",
                column: "PromocaoId");

            migrationBuilder.CreateIndex(
                name: "IX_JogosPromocoes_UsuarioId",
                table: "JogosPromocoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Promocao_UsuarioId",
                table: "Promocao",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioJogoPropriedade_JogoId",
                table: "UsuarioJogoPropriedade",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioJogoPropriedade_PromocaoId",
                table: "UsuarioJogoPropriedade",
                column: "PromocaoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioJogoPropriedade_UsuarioId",
                table: "UsuarioJogoPropriedade",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JogosPromocoes");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "UsuarioJogoPropriedade");

            migrationBuilder.DropTable(
                name: "Jogo");

            migrationBuilder.DropTable(
                name: "Promocao");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
