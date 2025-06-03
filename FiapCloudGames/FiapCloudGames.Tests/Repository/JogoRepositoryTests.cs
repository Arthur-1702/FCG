using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FiapCloudGames.Tests.Repository
{
    public class JogoRepositoryTests
    {
        private static Mock<DbSet<Jogo>> CriarMockDbSet(IEnumerable<Jogo> dados)
        {
            var queryable = dados.AsQueryable();
            var mockSet = new Mock<DbSet<Jogo>>();
            mockSet.As<IQueryable<Jogo>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Jogo>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Jogo>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Jogo>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static JogoRepository CriarRepositorioComDados(IEnumerable<Jogo> dados)
        {
            var mockSet = CriarMockDbSet(dados);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Jogo>()).Returns(mockSet.Object);
            return new JogoRepository(mockContext.Object);
        }

        [Fact]
        public void VerificarJogo_PorId_RetornaJogo_QuandoExiste()
        {
            var jogos = new List<Jogo>
            {
                new Jogo { Id = 1, Nome = "FIFA", Descricao = "Futebol", Preco = 100, UsuarioId = 1 },
                new Jogo { Id = 2, Nome = "NBA", Descricao = "Basquete", Preco = 120, UsuarioId = 2 }
            };
            var repo = CriarRepositorioComDados(jogos);

            var resultado = repo.CheckJogo(2);

            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(2);
            resultado.Nome.Should().Be("NBA");
        }

        [Fact]
        public void VerificarJogo_PorId_RetornaNull_QuandoNaoExiste()
        {
            var jogos = new List<Jogo>
            {
                new Jogo { Id = 1, Nome = "FIFA", Descricao = "Futebol", Preco = 100, UsuarioId = 1 }
            };
            var repo = CriarRepositorioComDados(jogos);

            var resultado = repo.CheckJogo(99);

            resultado.Should().BeNull();
        }

        [Fact]
        public void VerificarJogo_PorNome_RetornaJogo_QuandoExiste()
        {
            var jogos = new List<Jogo>
            {
                new Jogo { Id = 1, Nome = "FIFA", Descricao = "Futebol", Preco = 100, UsuarioId = 1 },
                new Jogo { Id = 2, Nome = "NBA", Descricao = "Basquete", Preco = 120, UsuarioId = 2 }
            };
            var repo = CriarRepositorioComDados(jogos);

            var resultado = repo.CheckJogo("NBA");

            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be("NBA");
        }

        [Fact]
        public void VerificarJogo_PorNome_RetornaNull_QuandoNaoExiste()
        {
            var jogos = new List<Jogo>
            {
                new Jogo { Id = 1, Nome = "FIFA", Descricao = "Futebol", Preco = 100, UsuarioId = 1 }
            };
            var repo = CriarRepositorioComDados(jogos);

            var resultado = repo.CheckJogo("PES");

            resultado.Should().BeNull();
        }

        [Fact]
        public void ObterTodosPorFiltro_RetornaJogosCorrespondentes()
        {
            var jogos = new List<Jogo>
            {
                new Jogo { Id = 1, Nome = "FIFA 2025", Descricao = "Futebol", Preco = 100, UsuarioId = 1 },
                new Jogo { Id = 2, Nome = "NBA 2025", Descricao = "Basquete", Preco = 120, UsuarioId = 2 },
                new Jogo { Id = 3, Nome = "FIFA Manager", Descricao = "Futebol", Preco = 80, UsuarioId = 3 }
            };
            var repo = CriarRepositorioComDados(jogos);

            var resultado = repo.GetTodosPorFiltro("FIFA");

            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(j => j.Nome.Contains("FIFA"));
        }

        [Fact]
        public void ObterTodosPorFiltro_RetornaVazio_QuandoNaoHaCorrespondencia()
        {
            var jogos = new List<Jogo>
            {
                new Jogo { Id = 1, Nome = "FIFA 2025", Descricao = "Futebol", Preco = 100, UsuarioId = 1 }
            };
            var repo = CriarRepositorioComDados(jogos);

            var resultado = repo.GetTodosPorFiltro("NBA");

            resultado.Should().BeEmpty();
        }
    }
}