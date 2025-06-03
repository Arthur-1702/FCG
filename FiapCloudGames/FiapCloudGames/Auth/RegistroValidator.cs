namespace FiapCloudGames.Api.Auth;

public class RegistroValidator
{
    public static bool EmailValido(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        try
        {
            var emailValido = new System.Net.Mail.MailAddress(email);
            return emailValido.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool SenhaValida(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;
        bool TemNumero = false;
        bool TemLetra = false;
        bool TemCaracterEspecial = false;
        foreach (char c in password)
        {
            if (char.IsDigit(c)) TemNumero = true;
            else if (char.IsLetter(c)) TemLetra = true;
            else if (!char.IsWhiteSpace(c)) TemCaracterEspecial = true;
        }
        return TemNumero && TemLetra && TemCaracterEspecial;
    }
}
