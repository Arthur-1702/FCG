using FiapCloudGames.Core.Entities;

namespace FiapCloudGames.Core.Interfaces.Repository
{
    public interface IJogoRepository : IRepository<Jogo>
    {    
        Jogo CheckJogo(int id);
        Jogo CheckJogo(string nome);
        List<Jogo> GetTodosPorFiltro(string filtroNome);
    }
}
