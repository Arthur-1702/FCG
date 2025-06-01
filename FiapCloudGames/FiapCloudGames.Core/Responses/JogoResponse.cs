namespace FiapCloudGames.Core.Responses
{
    public record JogoResponse
    (
        string Nome,
        string Descricao,
        decimal Preco,              
        string? Empresa
    );
}
