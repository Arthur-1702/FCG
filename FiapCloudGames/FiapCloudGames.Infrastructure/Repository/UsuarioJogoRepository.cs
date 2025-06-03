using FiapCloudGames.Domain.Interfaces.Repository;
using FiapCloudGames.Domain.Entities;

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
