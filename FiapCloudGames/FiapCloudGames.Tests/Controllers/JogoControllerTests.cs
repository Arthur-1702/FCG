using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FiapCloudGames.Tests.Controllers
{
    public class JogoControllerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepoMock;
        private readonly Mock<ILogger<JogoController>> _loggerMock;
        private readonly JogoController _controller;

        public JogoControllerTests()
        {
            _jogoRepoMock = new Mock<IJogoRepository>();
            _loggerMock = new Mock<ILogger<JogoController>>();
            _controller = new JogoController(_jogoRepoMock.Object, _loggerMock.Object);
        }

        private void DefinirUsuarioComId(int userId, string role = "Admin")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public void Get_DeveRetornarBadRequest_EmExcecao()
        {
            _jogoRepoMock.Setup(r => r.GetTodos()).Throws(new Exception("fail"));

            var resultado = _controller.Get(null) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void GetPorId_DeveRetornarJogoResponse_QuandoEncontrado()
        {
            var jogo = new Jogo { Id = 1, Nome = "FIFA", Genero = "Esporte", Descricao = "Futebol", Preco = 100 };
            _jogoRepoMock.Setup(r => r.GetPorId(1)).Returns(jogo);

            var resultado = _controller.Get(1) as OkObjectResult;

            resultado.Should().NotBeNull();
            resultado!.Value.Should().NotBeNull();
        }

        [Fact]
        public void GetPorId_DeveRetornarBadRequest_EmExcecao()
        {
            _jogoRepoMock.Setup(r => r.GetPorId(1)).Throws(new Exception("fail"));

            var resultado = _controller.Get(1) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Post_DeveCadastrarJogo_QuandoValido()
        {
            DefinirUsuarioComId(10);
            var input = new FiapCloudGames.Core.DTOs.JogoDTO
            {
                Nome = "FIFA",
                Genero = "Esporte",
                Descricao = "Futebol",
                Preco = 100
            };
            _jogoRepoMock.Setup(r => r.CheckJogo("FIFA")).Returns((Jogo)null);

            var resultado = _controller.Post(input) as OkObjectResult;

            resultado.Should().NotBeNull();
            _jogoRepoMock.Verify(r => r.Cadastrar(It.IsAny<Jogo>()), Times.Once);
        }

        [Fact]
        public void Post_DeveRetornarBadRequest_QuandoJogoJaExiste()
        {
            DefinirUsuarioComId(10);
            var input = new FiapCloudGames.Core.DTOs.JogoDTO
            {
                Nome = "FIFA",
                Genero = "Esporte",
                Descricao = "Futebol",
                Preco = 100
            };
            _jogoRepoMock.Setup(r => r.CheckJogo("FIFA")).Returns(new Jogo { Nome = "FIFA" });

            var resultado = _controller.Post(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Post_DeveRetornarBadRequest_EmExcecao()
        {
            DefinirUsuarioComId(10);
            var input = new FiapCloudGames.Core.DTOs.JogoDTO
            {
                Nome = "FIFA",
                Genero = "Esporte",
                Descricao = "Futebol",
                Preco = 100
            };
            _jogoRepoMock.Setup(r => r.CheckJogo("FIFA")).Throws(new Exception("fail"));

            var resultado = _controller.Post(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Put_DeveAtualizarJogo_QuandoValido()
        {
            DefinirUsuarioComId(10);
            var input = new FiapCloudGames.Core.DTOs.AtualizarJogoDTO
            {
                Id = 1,
                Nome = "FIFA",
                Genero = "Esporte",
                Descricao = "Futebol",
                Preco = 100
            };
            var jogo = new Jogo { Id = 1, Nome = "Old", Genero = "Old", Descricao = "Old", Preco = 50, UsuarioId = 10 };
            _jogoRepoMock.Setup(r => r.GetPorId(1)).Returns(jogo);

            var resultado = _controller.Put(input) as OkObjectResult;

            resultado.Should().NotBeNull();
            _jogoRepoMock.Verify(r => r.Atualizar(It.IsAny<Jogo>()), Times.Once);
        }

        [Fact]
        public void Put_DeveRetornarBadRequest_EmExcecao()
        {
            DefinirUsuarioComId(10);
            var input = new FiapCloudGames.Core.DTOs.AtualizarJogoDTO
            {
                Id = 1,
                Nome = "FIFA",
                Genero = "Esporte",
                Descricao = "Futebol",
                Preco = 100
            };
            _jogoRepoMock.Setup(r => r.GetPorId(1)).Throws(new Exception("fail"));

            var resultado = _controller.Put(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Delete_DeveDeletarJogo_QuandoValido()
        {
            DefinirUsuarioComId(10);
            var resultado = _controller.Delete(1) as OkObjectResult;

            resultado.Should().NotBeNull();
            _jogoRepoMock.Verify(r => r.Deletar(1), Times.Once);
        }

        [Fact]
        public void Delete_DeveRetornarBadRequest_EmExcecao()
        {
            DefinirUsuarioComId(10);
            _jogoRepoMock.Setup(r => r.Deletar(1)).Throws(new Exception("fail"));

            var resultado = _controller.Delete(1) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }
    }
}