using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Core.DTOs
{
    public class CadastroUsuarioJogoDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int JogoId { get; set; }

        public int PromocaoId { get; set; }
    }
}
