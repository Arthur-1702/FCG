using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;

namespace FiapCloudGames.Infrastructure.Repository
{
    public sealed class PromocaoRepository : EFREpository<Promocao>, IPromocaoRepository
    {
        public PromocaoRepository(ApplicationDbContext context) : base(context)
        { 
        }

        public bool TemPromocaoComNome(string nome) => 
            _dbSet.Any(p => p.Descricao.ToLower().Trim() == nome.ToLower().Trim());
    }

}
