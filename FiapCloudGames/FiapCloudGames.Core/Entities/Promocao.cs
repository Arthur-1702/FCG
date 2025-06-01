using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FiapCloudGames.Core.Entities
{
    public class Promocao : EntityBase
    {
       
        [Required]
        [MaxLength(100)]
        public required string Descricao { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [Required]
        public bool Ativo { get; set; }




        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; }

        [JsonIgnore]
        public virtual ICollection<JogosPromocoes>? JogosPromocoes { get; set; }

        [JsonIgnore]
        public virtual ICollection<UsuarioJogoPropriedade>? UsuarioJogos { get; set; }
    }
}
