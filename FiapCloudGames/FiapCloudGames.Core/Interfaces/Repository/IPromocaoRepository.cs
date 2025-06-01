using FiapCloudGames.Core.Entities;

namespace FiapCloudGames.Core.Interfaces.Repository
{
    public interface IPromocaoRepository : IRepository<Promocao>
    {
        bool TemPromocaoComNome(string nome);
    }
}
