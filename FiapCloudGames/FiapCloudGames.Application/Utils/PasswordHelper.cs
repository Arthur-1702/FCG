namespace FiapCloudGames.Application.Utils
{
    public static class PasswordHelper
    {
        public static string HashSenha(string senha) =>
            BCrypt.Net.BCrypt.HashPassword(senha);

        public static bool VerificarSenha(string senha, string hashSalvo) => 
            BCrypt.Net.BCrypt.Verify(senha, hashSalvo);
    }
}