namespace FiapCloudGames.Application.Responses
{
    public record JogoResponse
    (
        string Nome,
        string Descricao,
        decimal Preco,              
        string? Genero
    );
}
