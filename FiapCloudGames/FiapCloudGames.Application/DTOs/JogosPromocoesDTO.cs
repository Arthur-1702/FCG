namespace FiapCloudGames.Application.DTOs
{
    public record JogosPromocoesDTO(
        int JogoId,
        int PromocaoId,
        decimal Desconto
    );
}
