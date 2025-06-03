using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FiapCloudGames.Domain.Entities
{
    public class Usuario : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Senha { get; set; }

        [Required]
        [MaxLength(100)]
        public required string NivelAcesso { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Saldo { get; set; }



        [JsonIgnore]
        public virtual ICollection<UsuarioJogoPropriedade> JogosObtidos { get; set; }
        [JsonIgnore]
        public virtual ICollection<Promocao> Promocoes { get; set; }
        [JsonIgnore]
        public virtual ICollection<JogosPromocoes> JogosPromocoes { get; set; }
        [JsonIgnore]
        public virtual ICollection<Jogo> Jogos { get; set; }
    }
}
