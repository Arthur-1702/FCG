using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Repository
{
    public interface IRepository<T> where T : EntityBase
    {
        IList<T> GetTodos();
        T GetPorId(int id);
        void Cadastrar(T entidade);
        void Atualizar(T entidade);
        void Deletar(int id);
    }
}
