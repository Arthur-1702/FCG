using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Core.DTOs;
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
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<ILogger<UsuarioController>> _loggerMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _loggerMock = new Mock<ILogger<UsuarioController>>();
            _controller = new UsuarioController(_usuarioRepoMock.Object, _loggerMock.Object);
        }

        private void DefinirUsuarioLogado(Usuario usuario, string role = "Usuario")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "mock");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public void GetTodos_DeveRetornarUsuarios()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "João", Email = "joao@email.com", Senha = "123", NivelAcesso = "Usuario", Saldo = 10 }
            };
            _usuarioRepoMock.Setup(r => r.GetTodos()).Returns(usuarios);

            var resultado = _controller.GetTodos() as OkObjectResult;

            resultado.Should().NotBeNull();
            resultado!.Value.Should().NotBeNull();
        }

        [Fact]
        public void GetTodos_DeveRetornarBadRequest_EmExcecao()
        {
            _usuarioRepoMock.Setup(r => r.GetTodos()).Throws(new Exception("erro"));

            var resultado = _controller.GetTodos() as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Get_DeveRetornarUsuario_QuandoEncontrado()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", Email = "maria@email.com", Senha = "abc", NivelAcesso = "Usuario", Saldo = 20 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);

            var resultado = _controller.Get(1) as OkObjectResult;

            resultado.Should().NotBeNull();
            resultado!.Value.Should().NotBeNull();
        }

        [Fact]
        public void Get_DeveRetornarBadRequest_EmExcecao()
        {
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Throws(new Exception("erro"));

            var resultado = _controller.Get(1) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Post_DeveCadastrarUsuario_QuandoValido()
        {
            var input = new UsuarioDTO { Nome = "Novo", Email = "novo@email.com", Senha = "senha" };
            _usuarioRepoMock.Setup(r => r.GetPorEmail(input.Email)).Returns((Usuario)null);

            var resultado = _controller.Post(input) as OkObjectResult;

            resultado.Should().NotBeNull();
            _usuarioRepoMock.Verify(r => r.Cadastrar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public void Post_DeveRetornarBadRequest_QuandoEmailExistente()
        {
            var input = new UsuarioDTO { Nome = "Novo", Email = "existe@email.com", Senha = "senha" };
            _usuarioRepoMock.Setup(r => r.GetPorEmail(input.Email)).Returns(new Usuario { Email = input.Email, NivelAcesso = "Usuario" });

            var resultado = _controller.Post(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Post_DeveRetornarBadRequest_EmExcecao()
        {
            var input = new UsuarioDTO { Nome = "Novo", Email = "novo@email.com", Senha = "senha" };
            _usuarioRepoMock.Setup(r => r.GetPorEmail(input.Email)).Throws(new Exception("erro"));

            var resultado = _controller.Post(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Put_DeveAtualizarUsuario_QuandoValido()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", Email = "maria@email.com", Senha = "abc", NivelAcesso = "Usuario", Saldo = 20 };
            DefinirUsuarioLogado(usuario);
            var input = new AtualizarUsuarioDTO { Id = 1, Nome = "Maria Atualizada", Senha = "novaSenha" };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);

            var resultado = _controller.Put(input) as OkObjectResult;

            resultado.Should().NotBeNull();
            _usuarioRepoMock.Verify(r => r.Atualizar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public void Put_DeveRetornarBadRequest_QuandoUsuarioNaoExiste()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", Email = "maria@email.com", Senha = "abc", NivelAcesso = "Usuario", Saldo = 20 };
            DefinirUsuarioLogado(usuario);
            var input = new AtualizarUsuarioDTO { Id = 2, Nome = "Outro", Senha = "novaSenha" };
            _usuarioRepoMock.Setup(r => r.GetPorId(2)).Returns((Usuario)null);

            var resultado = _controller.Put(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Put_DeveRetornarUnauthorized_QuandoNaoAutenticado()
        {
            var input = new AtualizarUsuarioDTO { Id = 1, Nome = "Maria", Senha = "novaSenha" };
            // Não define usuário logado

            var resultado = _controller.Put(input) as ObjectResult;

            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(401);
        }

        [Fact]
        public void Put_DeveRetornarForbidden_QuandoUsuarioSemPermissao()
        {
            var usuarioLogado = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario" };
            var usuarioOutro = new Usuario { Id = 2, Nome = "Outro", NivelAcesso = "Usuario" };
            DefinirUsuarioLogado(usuarioLogado);
            var input = new AtualizarUsuarioDTO { Id = 2, Nome = "Outro", Senha = "novaSenha" };
            _usuarioRepoMock.Setup(r => r.GetPorId(2)).Returns(usuarioOutro);

            var resultado = _controller.Put(input) as ObjectResult;

            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(403);
        }

        [Fact]
        public void Put_DeveRetornarBadRequest_EmExcecao()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", Email = "maria@email.com", Senha = "abc", NivelAcesso = "Usuario", Saldo = 20 };
            DefinirUsuarioLogado(usuario);
            var input = new AtualizarUsuarioDTO { Id = 1, Nome = "Maria", Senha = "novaSenha" };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Throws(new Exception("erro"));

            var resultado = _controller.Put(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void TransformarEmAdmin_DeveAtualizarNivelAcesso()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario" };
            DefinirUsuarioLogado(new Usuario { Id = 99, Nome = "Admin", NivelAcesso = "Admin" }, "Admin");
            var input = new UsuarioAdminDTO { IdUsuario = 1 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);

            var resultado = _controller.TransformarEmAdmin(input) as OkObjectResult;

            resultado.Should().NotBeNull();
            _usuarioRepoMock.Verify(r => r.Atualizar(It.Is<Usuario>(u => u.NivelAcesso == "Admin")), Times.Once);
        }

        [Fact]
        public void TransformarEmAdmin_DeveRetornarBadRequest_QuandoUsuarioNaoExiste()
        {
            DefinirUsuarioLogado(new Usuario { Id = 99, Nome = "Admin", NivelAcesso = "Admin" }, "Admin");
            var input = new UsuarioAdminDTO { IdUsuario = 1 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns((Usuario)null);

            var resultado = _controller.TransformarEmAdmin(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void TransformarEmAdmin_DeveRetornarUnauthorized_QuandoNaoAutenticado()
        {
            var input = new UsuarioAdminDTO { IdUsuario = 1 };
            // Não define usuário logado

            var resultado = _controller.TransformarEmAdmin(input) as ObjectResult;

            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(401);
        }

        [Fact]
        public void TransformarEmAdmin_DeveRetornarBadRequest_EmExcecao()
        {
            DefinirUsuarioLogado(new Usuario { Id = 99, Nome = "Admin", NivelAcesso = "Admin" }, "Admin");
            var input = new UsuarioAdminDTO { IdUsuario = 1 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Throws(new Exception("erro"));

            var resultado = _controller.TransformarEmAdmin(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Delete_DeveDeletarUsuario_QuandoValido()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario" };
            DefinirUsuarioLogado(usuario);
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);

            var resultado = _controller.Delete(1) as OkObjectResult;

            resultado.Should().NotBeNull();
            _usuarioRepoMock.Verify(r => r.Deletar(1), Times.Once);
        }

        [Fact]
        public void Delete_DeveRetornarBadRequest_QuandoUsuarioNaoExiste()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario" };
            DefinirUsuarioLogado(usuario);
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns((Usuario)null);

            var resultado = _controller.Delete(1) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Delete_DeveRetornarUnauthorized_QuandoNaoAutenticado()
        {
            var resultado = _controller.Delete(1) as ObjectResult;

            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(401);
        }

        [Fact]
        public void Delete_DeveRetornarBadRequest_EmExcecao()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario" };
            DefinirUsuarioLogado(usuario);
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Throws(new Exception("erro"));

            var resultado = _controller.Delete(1) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Depositar_DeveAdicionarSaldo_QuandoValido()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario", Saldo = 10 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);
            _usuarioRepoMock.Setup(r => r.Depositar(1, 50)).Returns(60);

            var input = new UsuarioDeposito { Id = 1, Deposito = 50 };

            var resultado = _controller.Depositar(input) as OkObjectResult;

            resultado.Should().NotBeNull();
            _usuarioRepoMock.Verify(r => r.Depositar(1, 50), Times.Once);
        }

        [Fact]
        public void Depositar_DeveRetornarBadRequest_QuandoUsuarioNaoExiste()
        {
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns((Usuario)null);
            var input = new UsuarioDeposito { Id = 1, Deposito = 50 };

            var resultado = _controller.Depositar(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Depositar_DeveRetornarBadRequest_QuandoValorNegativo()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario", Saldo = 10 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);
            var input = new UsuarioDeposito { Id = 1, Deposito = -10 };

            var resultado = _controller.Depositar(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }

        [Fact]
        public void Depositar_DeveRetornarBadRequest_EmExcecao()
        {
            var usuario = new Usuario { Id = 1, Nome = "Maria", NivelAcesso = "Usuario", Saldo = 10 };
            _usuarioRepoMock.Setup(r => r.GetPorId(1)).Returns(usuario);
            _usuarioRepoMock.Setup(r => r.Depositar(1, 50)).Throws(new Exception("erro"));
            var input = new UsuarioDeposito { Id = 1, Deposito = 50 };

            var resultado = _controller.Depositar(input) as BadRequestObjectResult;

            resultado.Should().NotBeNull();
        }
    }
}