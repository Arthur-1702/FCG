namespace FiapCloudGames.Core.Responses
{
    public record UsuarioResponse
    (
        int UsuarioId,
        string Nome,
        string Email,
        string Senha,
        string NivelAcesso,
        decimal Saldo
    );
}
