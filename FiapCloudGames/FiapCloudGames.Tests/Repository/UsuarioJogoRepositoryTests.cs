using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FiapCloudGames.Tests.Repository
{
    public class UsuarioJogoRepositoryTests
    {
        private static Mock<DbSet<UsuarioJogoPropriedade>> CriarMockDbSet(IEnumerable<UsuarioJogoPropriedade> dados)
        {
            var queryable = dados.AsQueryable();
            var mockSet = new Mock<DbSet<UsuarioJogoPropriedade>>();
            mockSet.As<IQueryable<UsuarioJogoPropriedade>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<UsuarioJogoPropriedade>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<UsuarioJogoPropriedade>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<UsuarioJogoPropriedade>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static UsuarioJogoRepository CriarRepositorioComDados(IEnumerable<UsuarioJogoPropriedade> dados)
        {
            var mockSet = CriarMockDbSet(dados);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<UsuarioJogoPropriedade>()).Returns(mockSet.Object);
            return new UsuarioJogoRepository(mockContext.Object);
        }

        [Fact]
        public void GetPorIdUsuarioIdJogo_DeveRetornarPropriedade_QuandoExiste()
        {
            var propriedades = new List<UsuarioJogoPropriedade>
            {
                new UsuarioJogoPropriedade { UsuarioId = 1, JogoId = 2, ValorPago = 50 },
                new UsuarioJogoPropriedade { UsuarioId = 1, JogoId = 3, ValorPago = 60 }
            };
            var repo = CriarRepositorioComDados(propriedades);

            var resultado = repo.GetPorIdUsuarioIdJogo(1, 2);

            resultado.Should().NotBeNull();
            resultado!.UsuarioId.Should().Be(1);
            resultado.JogoId.Should().Be(2);
        }

        [Fact]
        public void GetPorIdUsuarioIdJogo_DeveRetornarNull_QuandoNaoExiste()
        {
            var propriedades = new List<UsuarioJogoPropriedade>
            {
                new UsuarioJogoPropriedade { UsuarioId = 1, JogoId = 2, ValorPago = 50 }
            };
            var repo = CriarRepositorioComDados(propriedades);

            var resultado = repo.GetPorIdUsuarioIdJogo(2, 3);

            resultado.Should().BeNull();
        }

        [Fact]
        public void GetJogosCompradosPorUsuario_DeveRetornarLista_QuandoUsuarioPossuiJogos()
        {
            var propriedades = new List<UsuarioJogoPropriedade>
            {
                new UsuarioJogoPropriedade { UsuarioId = 1, JogoId = 2, ValorPago = 50 },
                new UsuarioJogoPropriedade { UsuarioId = 1, JogoId = 3, ValorPago = 60 },
                new UsuarioJogoPropriedade { UsuarioId = 2, JogoId = 4, ValorPago = 70 }
            };
            var repo = CriarRepositorioComDados(propriedades);

            var resultado = repo.GetJogosCompradosPorUsuario(1);

            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(p => p.UsuarioId == 1);
        }

        [Fact]
        public void GetJogosCompradosPorUsuario_DeveRetornarListaVazia_QuandoUsuarioNaoPossuiJogos()
        {
            var propriedades = new List<UsuarioJogoPropriedade>
            {
                new UsuarioJogoPropriedade { UsuarioId = 2, JogoId = 4, ValorPago = 70 }
            };
            var repo = CriarRepositorioComDados(propriedades);

            var resultado = repo.GetJogosCompradosPorUsuario(1);

            resultado.Should().BeEmpty();
        }
    }
}