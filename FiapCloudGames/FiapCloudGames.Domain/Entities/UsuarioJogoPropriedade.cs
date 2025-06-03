using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FiapCloudGames.Domain.Entities
{
    public class UsuarioJogoPropriedade : EntityBase
    {
        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
                
        [Required]
        [ForeignKey("Jogo")]
        public int JogoId { get; set; }
        public virtual Jogo? Jogo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 0)")]
        public decimal ValorPago { get; set; }

        [ForeignKey("Promocao")]
        public int? PromocaoId { get; set; }

        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; }

        [JsonIgnore]
        public virtual Promocao? Promocao { get; set; }
    }
}
