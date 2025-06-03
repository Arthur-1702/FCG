using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Repository
{
    public sealed class JogosPromocoesRepository : EFREpository<JogosPromocoes>, IJogosPromocoesRepository
    {
        public JogosPromocoesRepository(ApplicationDbContext context) : base(context)
        {
        }
        public bool TemPromocaoAtiva(int jogoId)
        {
            var dataAtual = DateTime.Now;

            return _dbSet
                .Include(x => x.Promocao)
                .AsNoTracking()
                .Any(x => x.JogoId == jogoId &&
                x.Promocao.Ativo == true &&
                x.Promocao.DataInicio >= dataAtual &&
                x.Promocao.DataFim <= dataAtual);
        }

        public JogosPromocoes? GetPromocaoAtiva(int jogoId, int PromocaoId)
        {
            var dataAtual = DateTime.Now;

            return _dbSet
                .Include(x => x.Promocao)
                .AsNoTracking()
                .FirstOrDefault(x =>
                    x.JogoId == jogoId &&
                    x.Promocao.Ativo &&
                    x.Promocao.DataInicio <= dataAtual &&
                    x.Promocao.DataFim >= dataAtual &&
                    (PromocaoId == 0 || x.Promocao.Id == PromocaoId)
                );
        }
    }
}
