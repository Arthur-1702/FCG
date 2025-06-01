using FiapCloudGames.Core.Entities;

namespace FiapCloudGames.Core.Interfaces.Repository
{
    public interface IUsuarioJogoPropriedadeRepository : IRepository<UsuarioJogoPropriedade>
    {
        UsuarioJogoPropriedade GetPorIdUsuarioIdJogo(int idUsuario, int idJogo);

        List<UsuarioJogoPropriedade> GetJogosCompradosPorUsuario(int idUsuario);
    }
}
