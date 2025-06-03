using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Repository
{
    public class EFREpository<T> : IRepository<T> where T : EntityBase
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;

        public EFREpository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IList<T> GetTodos()
        {
            return _dbSet.ToList();
        }

        public T GetPorId(int id)
        {
            return _dbSet.FirstOrDefault(entity => entity.Id == id);
        }

        void IRepository<T>.Cadastrar(T entidade)
        {
            entidade.DataCriacao = DateTime.Now;
            _dbSet.Add(entidade);
            _context.SaveChanges();
        }

        void IRepository<T>.Atualizar(T entidade)
        {
            _dbSet.Update(entidade);
            _context.SaveChanges();
        }

        void IRepository<T>.Deletar(int id)
        {
            _dbSet.Remove(GetPorId(id));
            _context.SaveChanges();
        }
    }
}
