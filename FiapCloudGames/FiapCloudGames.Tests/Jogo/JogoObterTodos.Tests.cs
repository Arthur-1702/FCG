using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Core.DTOs;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FiapCloudGames.Tests
{
    public class JogoGetTodos
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<ILogger<JogoController>> _loggerMock;
        private readonly JogoController _controller;

        public JogoGetTodos()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _loggerMock = new Mock<ILogger<JogoController>>();
            _controller = new JogoController(_jogoRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Get_SemFiltro_DeveRetornarListaDeJogos()
        {
            // Arrange
            var jogos = new List<Jogo>
            {
                new() { Id = 1, Nome = "Minecraft", Genero = "Mojang", Descricao = "Blocos", Preco = 100,  DataCriacao = DateTime.Now },
                new() { Id = 2, Nome = "CS2", Genero = "Valve", Descricao = "FPS", Preco = 200, DataCriacao = DateTime.Now }
            };

            _jogoRepositoryMock.Setup(r => r.GetTodos()).Returns(jogos);

            // Act
            var resultado = _controller.Get(null) as OkObjectResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(200, resultado.StatusCode);
            var data = Assert.IsType<List<JogoDTO>>(resultado.Value);
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public void Get_ComFiltro_DeveRetornarJogosFiltrados()
        {
            // Arrange
            string filtro = "Minecraft";
            var jogos = new List<Jogo>
            {
                new() { Id = 1, Nome = "Minecraft", Genero = "Mojang", Descricao = "Blocos", Preco = 100, DataCriacao = DateTime.Now }
            };

            _jogoRepositoryMock.Setup(r => r.GetTodosPorFiltro(filtro)).Returns(jogos);

            // Act
            var resultado = _controller.Get(filtro) as OkObjectResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(200, resultado.StatusCode);
            var data = Assert.IsType<List<JogoDTO>>(resultado.Value);
            Assert.Single(data);
            Assert.Equal("Minecraft", data[0].Nome);
        }

        [Fact]
        public void Get_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            _jogoRepositoryMock.Setup(r => r.GetTodos()).Throws(new Exception("Erro inesperado"));

            // Act
            var resultado = _controller.Get(null) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(400, resultado.StatusCode);
            var response = Assert.IsType<ApiResponse<string>>(resultado.Value);
            Assert.False(response.Sucesso);
            Assert.Contains("Erro ao tentar trazer todos os Jogos", response.Erro!.Mensagem);
        }
    }
}
