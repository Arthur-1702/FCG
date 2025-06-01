using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;

namespace FiapCloudGames.Infrastructure.Repository
{
    public class UsuarioJogoRepository : EFREpository<UsuarioJogoPropriedade>, IUsuarioJogoPropriedadeRepository
    {
        public UsuarioJogoRepository(ApplicationDbContext context) : base(context)
        {
        }
        public UsuarioJogoPropriedade? GetPorIdUsuarioIdJogo(int idUsuario, int idJogo)
        {
            return _dbSet.FirstOrDefault(entity => entity.UsuarioId == idUsuario && entity.JogoId == idJogo);
        }

        public List<UsuarioJogoPropriedade> GetJogosCompradosPorUsuario(int idUsuario)
        {
            return _dbSet.Where(entity => entity.UsuarioId == idUsuario).ToList();
        }

    }
}
