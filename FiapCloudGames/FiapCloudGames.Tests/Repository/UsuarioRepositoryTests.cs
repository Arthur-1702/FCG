using FiapCloudGames.Core.Entities;
using FiapCloudGames.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FiapCloudGames.Tests.Repository
{
    public class UsuarioRepositoryTests
    {
        private static Mock<DbSet<Usuario>> CriarMockDbSet(IEnumerable<Usuario> dados)
        {
            var queryable = dados.AsQueryable();
            var mockSet = new Mock<DbSet<Usuario>>();
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static UsuarioRepository CriarRepositorio(IEnumerable<Usuario> dados)
        {
            var mockSet = CriarMockDbSet(dados);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Usuario>()).Returns(mockSet.Object);
            return new UsuarioRepository(mockContext.Object);
        }

        [Fact]
        public void GetPorEmail_DeveRetornarUsuario_QuandoEmailExiste()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "João", Email = "joao@email.com", Senha = "123", NivelAcesso = "User", Saldo = 100 }
            };
            var repo = CriarRepositorio(usuarios);

            var resultado = repo.GetPorEmail("joao@email.com");

            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be("João");
        }

        [Fact]
        public void GetPorEmail_DeveRetornarNull_QuandoEmailNaoExiste()
        {
            var usuarios = new List<Usuario>();
            var repo = CriarRepositorio(usuarios);

            var resultado = repo.GetPorEmail("naoexiste@email.com");

            resultado.Should().BeNull();
        }

        [Fact]
        public void GetPorEmailESenha_DeveRetornarUsuario_QuandoEmailESenhaCorretos()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "Maria", Email = "maria@email.com", Senha = "senha123", NivelAcesso = "User", Saldo = 50 }
            };
            var repo = CriarRepositorio(usuarios);

            var resultado = repo.GetPorEmailESenha("maria@email.com", "senha123");

            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be("Maria");
        }

        [Fact]
        public void GetPorEmailESenha_DeveRetornarNull_QuandoDadosInvalidos()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "Maria", Email = "maria@email.com", Senha = "senha123", NivelAcesso = "User", Saldo = 50 }
            };
            var repo = CriarRepositorio(usuarios);

            var resultado = repo.GetPorEmailESenha("maria@email.com", "errada");

            resultado.Should().BeNull();
        }

        [Fact]
        public void ConferirSaldo_DeveRetornarSaldoCorreto()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "Carlos", Email = "carlos@email.com", Senha = "abc", NivelAcesso = "User", Saldo = 200 }
            };
            var repo = CriarRepositorio(usuarios);

            var saldo = repo.ConferirSaldo(1);

            saldo.Should().Be(200);
        }

        [Fact]
        public void Depositar_DeveAdicionarValorAoSaldo()
        {
            var usuario = new Usuario { Id = 1, Nome = "Ana", Email = "ana@email.com", Senha = "123", NivelAcesso = "User", Saldo = 100 };
            var usuarios = new List<Usuario> { usuario };
            var mockSet = CriarMockDbSet(usuarios);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Usuario>()).Returns(mockSet.Object);
            var repo = new UsuarioRepository(mockContext.Object);

            var novoSaldo = repo.Depositar(1, 50);

            novoSaldo.Should().Be(150);
            usuario.Saldo.Should().Be(150);
            mockSet.Verify(m => m.Update(It.IsAny<Usuario>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Subtrair_DeveSubtrairValorDoSaldo()
        {
            var usuario = new Usuario { Id = 1, Nome = "Ana", Email = "ana@email.com", Senha = "123", NivelAcesso = "User", Saldo = 100 };
            var usuarios = new List<Usuario> { usuario };
            var mockSet = CriarMockDbSet(usuarios);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Usuario>()).Returns(mockSet.Object);
            var repo = new UsuarioRepository(mockContext.Object);

            var novoSaldo = repo.Subtrair(1, 30);

            novoSaldo.Should().Be(70);
            usuario.Saldo.Should().Be(70);
            mockSet.Verify(m => m.Update(It.IsAny<Usuario>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Login_DeveRetornarNull_QuandoUsuarioNaoExiste()
        {
            var usuarios = new List<Usuario>();
            var mockSet = CriarMockDbSet(usuarios);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Usuario>()).Returns(mockSet.Object);

            var repo = new UsuarioRepository(mockContext.Object);

            var resultado = repo.Login("naoexiste@email.com", "senha");

            resultado.Should().BeNull();
        }
    }

    // Mock estático para PasswordHelper.VerificarSenha
    public static class PasswordHelperMocker
    {
        private static bool _retorno = true;

        public static void MockVerificarSenha(bool retorno)
        {
            _retorno = retorno;
            PasswordHelper.VerificarSenha = (senhaTexto, hash) => _retorno;
        }
    }

    // Classe estática para simular o PasswordHelper
    public static class PasswordHelper
    {
        public static Func<string, string, bool> VerificarSenha = (senhaTexto, hash) => senhaTexto == hash;
    }
}