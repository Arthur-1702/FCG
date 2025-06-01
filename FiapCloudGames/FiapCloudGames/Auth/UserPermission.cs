using FiapCloudGames.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FiapCloudGames.Api.Auth
{
    public class UserPermission : ControllerBase
    {
        public Usuario GetUsuarioLogado()
        {
            return new Usuario
            {
                Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                NivelAcesso = User.FindFirst(ClaimTypes.Role)?.Value ?? "Usuario"
            };
        }

        public void Admin(Usuario usuario)
        {
            Usuario usuarioLogado = GetUsuarioLogado();

            if (usuario == null)
                BadRequest("Usuário não cadastrado");

            bool admin = usuarioLogado.NivelAcesso == "Admin";
            bool mesmoUsuario = usuarioLogado.Id != usuario.Id;

            if (!admin && mesmoUsuario)
                Forbid("Acesso negado.");
        }
    }
}
