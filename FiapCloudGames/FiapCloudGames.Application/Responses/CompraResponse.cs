namespace FiapCloudGames.Application.Responses
{
    public record CompraResponse
    (
        int UsuarioId,
        int JogoId,
        string NomeJogo,
        decimal ValorPago,
        int? PromocaoId,
        decimal? DescontoAplicado,
        string Mensagem
    );
}
