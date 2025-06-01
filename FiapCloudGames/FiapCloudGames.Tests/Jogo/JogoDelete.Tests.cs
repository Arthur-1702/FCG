using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FiapCloudGames.Tests;
public class JogoDelete
{
    [Fact]
    public void Delete_ValidId_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var mockRepo = new Mock<IJogoRepository>();
        var mockLogger = new Mock<ILogger<JogoController>>();

        mockRepo.Setup(r => r.Deletar(1));

        var controller = new JogoController(mockRepo.Object, mockLogger.Object);

        // Act
        var result = controller.Delete(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);

        Assert.True(response.Sucesso);
        Assert.Contains("deletado com sucesso", response.Dados);

        mockRepo.Verify(r => r.Deletar(1), Times.Once);
    }

    [Fact]
    public void Delete_WhenExceptionThrown_ReturnsBadRequest()
    {
        // Arrange
        var mockRepo = new Mock<IJogoRepository>();
        var mockLogger = new Mock<ILogger<JogoController>>();

        mockRepo.Setup(r => r.Deletar(99)).Throws(new Exception("Erro ao deletar"));

        var controller = new JogoController(mockRepo.Object, mockLogger.Object);

        // Simula usuário autenticado com papel Admin
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"))
            }
        };

        // Act
        var result = controller.Delete(99);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(badRequest.Value);

        Assert.False(response.Sucesso);
        Assert.Equal(500, response.Erro.StatusCode);
        Assert.Contains("erro ocorreu ao tentar deletar", response.Erro.Mensagem);

        mockRepo.Verify(r => r.Deletar(99), Times.Once);
    }

}



