namespace FiapCloudGames.Core.DTOs
{
    public class AtualizarJogoDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Genero { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
