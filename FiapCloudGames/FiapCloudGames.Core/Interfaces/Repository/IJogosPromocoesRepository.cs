using FiapCloudGames.Core.Entities;

namespace FiapCloudGames.Core.Interfaces.Repository
{
    public interface IJogosPromocoesRepository : IRepository<JogosPromocoes>
    {
        bool TemPromocaoAtiva(int jogoId);
        JogosPromocoes GetPromocaoAtiva(int jogoId, int PromocaoId);
    }
}
