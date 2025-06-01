using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Core.DTOs;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FiapCloudGames.Tests;
public class JogoCreateTest
{
    private readonly Mock<IJogoRepository> _jogoRepositoryMock;
    private readonly Mock<ILogger<JogoController>> _loggerMock;
    private readonly JogoController _controller;

    public JogoCreateTest()
    {
        _jogoRepositoryMock = new Mock<IJogoRepository>();
        _loggerMock = new Mock<ILogger<JogoController>>();
        _controller = new JogoController(_jogoRepositoryMock.Object, _loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "1"),            
            new Claim(ClaimTypes.Role, "Admin")

        ], "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public void Post_DeveCadastrarJogo_QuandoDadosSaoValidos()
    {
        // Arrange
        var input = new JogoDTO
        {
            Nome = "Visual Studio",
            Genero = "Terror",
            Descricao = "Mão no código",
            Preco = 10.90m
        };

        _jogoRepositoryMock.Setup(r => r.Cadastrar(It.IsAny<Jogo>()));

        // Act
        var resultado = _controller.Post(input);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var jogo = okResult.Value as Jogo;
        jogo.Should().NotBeNull();
        jogo!.Nome.Should().Be("Visual Studio");
        jogo.Genero.Should().Be("Terror");
        jogo.Preco.Should().Be(199.90m);
        jogo.Descricao.Should().Be("Mão no código");
        jogo.UsuarioId.Should().Be(1);

        _jogoRepositoryMock.Verify(r => r.Cadastrar(It.IsAny<Jogo>()), Times.Once);
    }

    [Fact]
    public void Post_DeveRetornarBadRequest_QuandoJogoJaExiste()
    {
        // Arrange
        var input = new JogoDTO
        {
            Nome = "Visual Studio",
            Genero = "Terror",
            Descricao = "Mão no código",
            Preco = 10.90m
        };

        _jogoRepositoryMock
            .Setup(r => r.Cadastrar(It.IsAny<Jogo>()))
            .Throws(new Exception("O jogo já existe em nossos servidores"));

        // Act
        var resultado = _controller.Post(input);

        // Assert
        var badRequest = resultado as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        badRequest.Value.Should().Be($"O jogo {input.Nome} já existe em nossos servidores.");

        _jogoRepositoryMock.Verify(r => r.Cadastrar(It.IsAny<Jogo>()), Times.Once);
    }

    [Fact]
    public void Post_DeveRetornarErro400_SeOcorrerErroInesperado()
    {
        // Arrange
        var input = new JogoDTO
        {
            Nome = "Error",
            Genero = "Error",
            Descricao = "Error",
            Preco = 100
        };

        _jogoRepositoryMock
            .Setup(r => r.Cadastrar(It.IsAny<Jogo>()))
            .Throws(new Exception("Erro inesperado"));

        // Act
        var resultado = _controller.Post(input);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var response = objectResult.Value as ApiResponse<string>;
        response.Should().NotBeNull();
        response!.Erro.Should().NotBeNull();
        response!.Erro!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        response.Erro.Mensagem.Should().Contain($"Um erro ocorreu ao tentar cadastrar o jogo");

        _jogoRepositoryMock.Verify(r => r.Cadastrar(It.IsAny<Jogo>()), Times.Once);
    }
}
