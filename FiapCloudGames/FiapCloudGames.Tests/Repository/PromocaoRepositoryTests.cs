using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FiapCloudGames.Tests.Repository
{
    public class PromocaoRepositoryTests
    {
        private static Mock<DbSet<Promocao>> CriarMockDbSet(IEnumerable<Promocao> dados)
        {
            var queryable = dados.AsQueryable();
            var mockSet = new Mock<DbSet<Promocao>>();
            mockSet.As<IQueryable<Promocao>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Promocao>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Promocao>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Promocao>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static PromocaoRepository CriarRepositorioComDados(IEnumerable<Promocao> dados)
        {
            var mockSet = CriarMockDbSet(dados);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<Promocao>()).Returns(mockSet.Object);
            return new PromocaoRepository(mockContext.Object);
        }

        [Fact]
        public void TemPromocaoComNome_DeveRetornarTrue_QuandoDescricaoIgual()
        {
            var promocoes = new List<Promocao>
            {
                new Promocao { Id = 1, Descricao = "Promo��o de Ver�o" },
                new Promocao { Id = 2, Descricao = "Black Friday" }
            };
            var repo = CriarRepositorioComDados(promocoes);

            var resultado = repo.TemPromocaoComNome("promo��o de ver�o");

            resultado.Should().BeTrue();
        }

        [Fact]
        public void TemPromocaoComNome_DeveRetornarTrue_QuandoDescricaoComEspacosEDiferenteCase()
        {
            var promocoes = new List<Promocao>
            {
                new Promocao { Id = 1, Descricao = "  Black Friday  " }
            };
            var repo = CriarRepositorioComDados(promocoes);

            var resultado = repo.TemPromocaoComNome("black friday");

            resultado.Should().BeTrue();
        }

        [Fact]
        public void TemPromocaoComNome_DeveRetornarFalse_QuandoDescricaoNaoExiste()
        {
            var promocoes = new List<Promocao>
            {
                new Promocao { Id = 1, Descricao = "Promo��o de Natal" }
            };
            var repo = CriarRepositorioComDados(promocoes);

            var resultado = repo.TemPromocaoComNome("Black Friday");

            resultado.Should().BeFalse();
        }
    }
}