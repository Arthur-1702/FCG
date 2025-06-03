namespace FiapCloudGames.Application.DTOs
{
    public class PromocaoDTO
    {
        public string Descricao { get; set; } = null!;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Ativo { get; set; }
    }
}
