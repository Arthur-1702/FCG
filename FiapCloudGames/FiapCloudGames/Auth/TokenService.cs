using FiapCloudGames.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiapCloudGames.Api.Auth;

public class TokenService
{
    private readonly JwtSettings _jwt;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwt = jwtSettings.Value;
    }

    public string GerarToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome ?? ""),
            new Claim(ClaimTypes.Role, usuario.NivelAcesso ?? "Usuario")
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.ChaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
