using FiapCloudGames.Api.Auth;
using FiapCloudGames.Core.DTOs;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using FiapCloudGames.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioRepository usuarioRepository,
                              ILogger<UsuarioController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        //GetTodos
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<UsuarioResponse>>), StatusCodes.Status200OK)]
        public IActionResult GetTodos()
        {
            try
            {
                List<Usuario> usuarios = _usuarioRepository.GetTodos().ToList();
                List<UsuarioResponse> response = usuarios.Select(u => new UsuarioResponse
                (
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Senha,
                    u.NivelAcesso,
                    u.Saldo
                )).ToList();
                return Ok(ApiResponse<List<UsuarioResponse>>.Ok(response));
            }
            catch (Exception ex)
            {
                string mensagem = "Erro ao trazer todos os usuários.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(mensagem);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), StatusCodes.Status200OK)]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                Usuario usuario = _usuarioRepository.GetPorId(id);
                UsuarioResponse response = new
                (
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Senha,
                    usuario.NivelAcesso,
                    usuario.Saldo
                );

                return Ok(ApiResponse<UsuarioResponse>.Ok(response));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao trazer usuario Id: {id}."; 
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(mensagem);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] UsuarioDTO input)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nome = input.Nome,
                    Email = input.Email,
                    Senha = PasswordHelper.HashSenha(input.Senha),
                    NivelAcesso = "Usuario",
                    Saldo = 0
                };

                if (_usuarioRepository.GetPorEmail(input.Email) != null)
                    return BadRequest($"Email \"{input.Email}\" já existente.");
                
                _usuarioRepository.Cadastrar(usuario);
                _logger.LogInformation($"Usuário \"{usuario.Nome}\" com email \"{usuario.Email}\" cadastrado com sucesso!");
                return Ok(ApiResponse<Usuario>.Ok(usuario));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao cadastrar usuario de email: {input.Email}";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(mensagem);
            }
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), StatusCodes.Status200OK)]
        public IActionResult Put([FromBody] AtualizarUsuarioDTO input)
        {
            try
            {
                var validacao = ValidarAlteracaoUsuario(input.Id, out var usuario);
                if (validacao != null)
                    return validacao;

                usuario.Nome = input.Nome;
                usuario.Senha = PasswordHelper.HashSenha(input.Senha);

                _usuarioRepository.Atualizar(usuario);

                UsuarioResponse response = new
                (
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Senha,
                    usuario.NivelAcesso,
                    usuario.Saldo
                );

                _logger.LogInformation($"Usuário \"{input.Nome}\" alterado com sucesso!");
                return Ok(ApiResponse<UsuarioResponse>.Ok(response));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao atualizar usuario: {input.Nome}.";
                _logger.LogError(mensagem + "Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        [HttpPut("administrador")]
        [Authorize(Roles = "Admin")]
        public IActionResult TransformarEmAdmin([FromBody] UsuarioAdminDTO input)
        {
            try
            {
                var validacao = ValidarAlteracaoUsuario(input.IdUsuario, out var usuario);
                if (validacao != null)
                    return validacao;

                usuario.NivelAcesso = "Admin";

                _usuarioRepository.Atualizar(usuario);

                _logger.LogInformation($"Usuário \"{usuario.Nome}\" tem cargo Administrador");

                return Ok(ApiResponse<Usuario>.Ok(usuario));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao atualizar o usuário Id: {input.IdUsuario}.";
                _logger.LogError(mensagem + "Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var validacao = ValidarAlteracaoUsuario(id, out var usuario);
                if (validacao != null)
                    return validacao;

                _usuarioRepository.Deletar(id);
                string mensagem = $"Usuário \"{usuario.Nome}\" deletado com sucesso!";
                _logger.LogInformation(mensagem);
                return Ok(ApiResponse<string>.Ok(mensagem));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao deletar o usuário de Id: {id}.";
                _logger.LogError(mensagem + "Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        [HttpPost("depositar")]
        public IActionResult Depositar([FromBody] UsuarioDeposito input)
        {
            try
            {
                if (_usuarioRepository.GetPorId(input.Id) == null)
                    return BadRequest("Usuário inexistente.");

                if (input.Deposito < 0)
                    return BadRequest($"Valor de R$ {input.Deposito} inválido.");

                var saldo = _usuarioRepository.Depositar(input.Id, input.Deposito);
                string mensagem = $"Adicionados R${input.Deposito} na conta do usuário. Saldo atual: R${saldo}";
                _logger.LogInformation(mensagem);

                return Ok(ApiResponse<string>.Ok(mensagem));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao depositar {input.Deposito} pro usuário de Id: {input.Id}.";
                _logger.LogError(mensagem + "Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        private static bool NaoEhAdminEQuerEditarOutroUsuario(int idUsuario, Usuario usuarioLogado, bool ehAdmin)
        {
            return !ehAdmin && usuarioLogado.Id != idUsuario;
        }

        private IActionResult? ValidarAlteracaoUsuario(int idUsuario, out Usuario usuario)
        {
            var usuarioLogado = UserInfo.GetUsuarioLogado(User);

            if (usuarioLogado == null)
            {
                usuario = null!;
                return Unauthorized(ApiResponse<string>.Error(401, "Usuário não autenticado."));
            }

            usuario = _usuarioRepository.GetPorId(idUsuario);
            if (usuario == null)
            {
                return BadRequest(ApiResponse<string>.Error(400, "Usuário não cadastrado."));
            }

            var admin = usuarioLogado.NivelAcesso == "Admin";

            if (NaoEhAdminEQuerEditarOutroUsuario(idUsuario, usuarioLogado, admin))
            {
                return StatusCode(403, ApiResponse<string>.Error(403, "Você não tem essa permissão."));
            }

            return null;
        }
    }
}
