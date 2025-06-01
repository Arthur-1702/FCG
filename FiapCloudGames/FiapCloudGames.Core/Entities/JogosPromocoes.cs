using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FiapCloudGames.Core.Entities
{
    public class JogosPromocoes: EntityBase
    {        

        [Required]
        [ForeignKey("Jogo")]
        public int JogoId { get; set; }
        public virtual Jogo Jogo { get; set; }

        [Required]
        [ForeignKey("Promocao")]
        public int PromocaoId { get; set; }
        public virtual Promocao Promocao { get; set; }
                
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Desconto { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }
    }
}
