using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Repository
{
    public interface IJogosPromocoesRepository : IRepository<JogosPromocoes>
    {
        bool TemPromocaoAtiva(int jogoId);
        JogosPromocoes GetPromocaoAtiva(int jogoId, int PromocaoId);
    }
}
