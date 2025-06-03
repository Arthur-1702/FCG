using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Application.DTOs
{
    public class CadastroUsuarioDTO
    {
        public required string Nome { get; set; }
        [EmailAddress(ErrorMessage = "Email invalido")]
        public required string Email { get; set; }
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$",
        ErrorMessage = "Senha com minimo de 8 caracteres, com um numero, uma letra maiuscula, uma letra minuscula e um caractere especial.")]
        public required string Senha { get; set; }
    }
}
