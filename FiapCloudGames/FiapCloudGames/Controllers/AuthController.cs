using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Domain.Interfaces.Repository;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _repository;
        private readonly TokenService _tokenService;

        public AuthController(IUsuarioRepository repository, TokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO input)
        {
            var usuario = _repository.Login(input.Email!, input.Senha!);
            if (usuario == null)
                return Unauthorized("Email/Senha inválidos");

            var token = _tokenService.GerarToken(usuario);
            return Ok(new { token });
        }
    }
}
