using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FiapCloudGames.Tests.Repository
{
    public class JogosPromocoesRepositoryTests
    {
        private static Mock<DbSet<JogosPromocoes>> CriarMockDbSet(IEnumerable<JogosPromocoes> dados)
        {
            var queryable = dados.AsQueryable();
            var mockSet = new Mock<DbSet<JogosPromocoes>>();
            mockSet.As<IQueryable<JogosPromocoes>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<JogosPromocoes>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<JogosPromocoes>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<JogosPromocoes>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }

        private static JogosPromocoesRepository CriarRepositorioComDados(IEnumerable<JogosPromocoes> dados)
        {
            var mockSet = CriarMockDbSet(dados);
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Set<JogosPromocoes>()).Returns(mockSet.Object);
            return new JogosPromocoesRepository(mockContext.Object);
        }

        [Fact]
        public void TemPromocaoAtiva_DeveRetornarFalse_QuandoNaoHaPromocaoAtiva()
        {
            var agora = DateTime.Now;
            var promocao = new Promocao
            {
                Id = 1,
                Descricao = "Promoção Inativa",
                Ativo = false,
                DataInicio = agora.AddMinutes(-20),
                DataFim = agora.AddMinutes(-10),
                UsuarioId = 1
            };
            var jogosPromocoes = new List<JogosPromocoes>
            {
                new JogosPromocoes
                {
                    JogoId = 1,
                    Promocao = promocao,
                    PromocaoId = promocao.Id,
                    Desconto = 10,
                    UsuarioId = 1
                }
            };
            var repo = CriarRepositorioComDados(jogosPromocoes);

            var resultado = repo.TemPromocaoAtiva(1);

            resultado.Should().BeFalse();
        }

        [Fact]
        public void GetPromocaoAtiva_DeveRetornarPromocao_QuandoExistePromocaoAtiva()
        {
            var agora = DateTime.Now;
            var promocao = new Promocao
            {
                Id = 2,
                Descricao = "Promoção Ativa",
                Ativo = true,
                DataInicio = agora.AddMinutes(-10),
                DataFim = agora.AddMinutes(10),
                UsuarioId = 1
            };
            var jogosPromocoes = new List<JogosPromocoes>
            {
                new JogosPromocoes
                {
                    JogoId = 1,
                    Promocao = promocao,
                    PromocaoId = promocao.Id,
                    Desconto = 20,
                    UsuarioId = 1
                }
            };
            var repo = CriarRepositorioComDados(jogosPromocoes);

            var resultado = repo.GetPromocaoAtiva(1, 2);

            resultado.Should().NotBeNull();
            resultado!.PromocaoId.Should().Be(2);
        }

        [Fact]
        public void GetPromocaoAtiva_DeveRetornarNull_QuandoNaoExistePromocaoAtiva()
        {
            var agora = DateTime.Now;
            var promocao = new Promocao
            {
                Id = 3,
                Descricao = "Promoção Expirada",
                Ativo = true,
                DataInicio = agora.AddMinutes(-30),
                DataFim = agora.AddMinutes(-10),
                UsuarioId = 1
            };
            var jogosPromocoes = new List<JogosPromocoes>
            {
                new JogosPromocoes
                {
                    JogoId = 1,
                    Promocao = promocao,
                    PromocaoId = promocao.Id,
                    Desconto = 15,
                    UsuarioId = 1
                }
            };
            var repo = CriarRepositorioComDados(jogosPromocoes);

            var resultado = repo.GetPromocaoAtiva(1, 3);

            resultado.Should().BeNull();
        }
    }
}