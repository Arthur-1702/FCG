using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repository;

namespace FiapCloudGames.Infrastructure.Repository
{
    public sealed class JogoRepository : EFREpository<Jogo>, IJogoRepository
    {
        public JogoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Jogo? CheckJogo(int id) =>
            _dbSet.FirstOrDefault(entity => entity.Id == id);

        public Jogo? CheckJogo(string nome) =>
            _dbSet.FirstOrDefault(entity => entity.Nome == nome);
  
        public List<Jogo> GetTodosPorFiltro(string filtroNome) =>
            _dbSet.Where(x => x.Nome.Contains(filtroNome)).ToList();
    }
}
