namespace FiapCloudGames.Application.DTOs
{
    public class JogoDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Genero { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }        
        public DateTime DataCriacao { get; set; }
    }
}
