using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FiapCloudGames.Core.Entities
{
    public class Jogo : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }                

        public string? Genero { get; set; }



        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<UsuarioJogoPropriedade> Proprietarios { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<JogosPromocoes> Promocoes { get; set; }

    }
}
