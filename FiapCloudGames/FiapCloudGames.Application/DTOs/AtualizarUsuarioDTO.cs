namespace FiapCloudGames.Application.DTOs
{
    public class AtualizarUsuarioDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Senha { get; set; }
    }
}
