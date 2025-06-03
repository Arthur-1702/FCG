using System.Security.Claims;
using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Api.Auth 
{ 
    public static class UserInfo
    {
        public static Usuario? GetUsuarioLogado(ClaimsPrincipal user)
        {
            if (UsuarioNaoEstaAutenticado(user))
                return null;

            var nameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nivelClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(nameIdentifier) || 
                !int.TryParse(nameIdentifier, out var id))
                return null;

            return new Usuario
            {
                Id = id,
                NivelAcesso = nivelClaim ?? "Usuario"            
            };
        }

        private static bool UsuarioNaoEstaAutenticado(ClaimsPrincipal user)
        {
            return user == null || user.Identity == null || !user.Identity.IsAuthenticated;
        }
    }
}