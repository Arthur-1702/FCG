using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Core.DTOs;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FiapCloudGames.Tests;

public class JogoUpdateTest
{
    private readonly Mock<IJogoRepository> _mockRepo;
    private readonly Mock<ILogger<JogoController>> _mockLogger;
    private readonly JogoController _controller;

    public JogoUpdateTest()
    {
        _mockRepo = new Mock<IJogoRepository>();
        _mockLogger = new Mock<ILogger<JogoController>>();
        _controller = new JogoController(_mockRepo.Object, _mockLogger.Object);
                
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Name, "Administrador")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public void Put_ValidInput_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var jogoOriginal = new Jogo
        {
            Id = 1,
            Nome = "Antigo Jogo",
            Genero = "Genero Antigo",
            Descricao = "Antiga descrição",
            Preco = 100,
            UsuarioId = 999
        };

        var input = new AtualizarJogoDTO
        {
            Id = 1,
            Nome = "Novo Jogo",
            Genero = "Genero Atual",
            Descricao = "Nova descrição",
            Preco = 150
        };

        _mockRepo.Setup(r => r.GetPorId(1)).Returns(jogoOriginal);
        _mockRepo.Setup(r => r.Atualizar(It.IsAny<Jogo>()));

        // Act
        var result = _controller.Put(input);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Contains("atualizado com sucesso", response.Dados);

        _mockRepo.Verify(r => r.Atualizar(It.Is<Jogo>(
            j => j.Id == 1 &&
                 j.Nome == "Novo Nome" &&
                 j.Genero == "Nova Genero" &&
                 j.Descricao == "Nova descrição" &&
                 j.Preco == 150 &&
                 j.UsuarioId == 1
        )), Times.Once);
    }

    [Fact]
    public void Put_ThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var input = new AtualizarJogoDTO
        {
            Id = 1,
            Nome = "Erro",
            Genero = "X",
            Descricao = "Teste",
            Preco = 10
        };

        _mockRepo.Setup(r => r.GetPorId(It.IsAny<int>()))
                 .Throws(new Exception("Erro de banco"));

        // Act
        var result = _controller.Put(input);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(badRequest.Value);

        Assert.Equal(500, response.Erro.StatusCode);
        Assert.Contains("Um erro ocorreu ao tentar atualizar o jogo", response.Erro.Mensagem);
    }
}
