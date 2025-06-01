namespace FiapCloudGames.Core.DTOs
{
    public record JogosPromocoesDTO(
        int JogoId,
        int PromocaoId,
        decimal Desconto
    );
}
