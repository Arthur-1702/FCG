using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Repository
{
    public interface IPromocaoRepository : IRepository<Promocao>
    {
        bool TemPromocaoComNome(string nome);
    }
}
