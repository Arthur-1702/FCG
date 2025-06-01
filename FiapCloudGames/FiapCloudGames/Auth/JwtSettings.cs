namespace FiapCloudGames.Api.Auth;

public class JwtSettings
{
    public string ChaveSecreta { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
}
