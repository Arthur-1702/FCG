using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Repository
{
    public interface IUsuarioJogoPropriedadeRepository : IRepository<UsuarioJogoPropriedade>
    {
        UsuarioJogoPropriedade GetPorIdUsuarioIdJogo(int idUsuario, int idJogo);

        List<UsuarioJogoPropriedade> GetJogosCompradosPorUsuario(int idUsuario);
    }
}