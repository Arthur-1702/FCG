using FiapCloudGames.Api.Auth;
using FiapCloudGames.Core.DTOs;
using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JogosPromocoesController : ControllerBase
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IPromocaoRepository _promocaoRepository;
        private readonly IJogosPromocoesRepository _jogosPromocoesRepository;
        private readonly ILogger<JogosPromocoesController> _logger;

        public JogosPromocoesController(
            IJogoRepository jogoRepository, 
            IPromocaoRepository promocaoRepository,
            IJogosPromocoesRepository jogosPromocoesRepository,
            ILogger<JogosPromocoesController> logger)
        {
            _jogoRepository = jogoRepository;
            _promocaoRepository = promocaoRepository;
            _jogosPromocoesRepository = jogosPromocoesRepository;
            _logger = logger;
        }

        [HttpPost("Cadastrar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult Cadastrar([FromBody] JogosPromocoesDTO input)
        {
            try
            {
                var usuario = UserInfo.GetUsuarioLogado(User);

                if (usuario is null)
                {
                    _logger.LogWarning("Usuário não autorizado.");
                    return Unauthorized(ApiResponse<string>.Error(StatusCodes.Status401Unauthorized, "Usuário não autorizado."));
                }

                if (_jogoRepository.GetPorId(input.JogoId) is null)
                {
                    _logger.LogWarning("Jogo não encontrado. JogoId: {JogoId}", input.JogoId);
                    return NotFound(ApiResponse<string>.Error(StatusCodes.Status404NotFound, "Jogo não encontrado."));
                }

                if (_promocaoRepository.GetPorId(input.PromocaoId) is null)
                {
                    _logger.LogWarning("Promoção não encontrada. PromocaoId: {PromocaoId}", input.PromocaoId);
                    return NotFound(ApiResponse<string>.Error(StatusCodes.Status404NotFound, "Promoção não encontrada."));
                }

                if (_jogosPromocoesRepository.TemPromocaoAtiva(input.JogoId))
                {
                    _logger.LogWarning("Já existe uma promoção ativa para o jogo. JogoId: {JogoId}", input.JogoId);
                    return Conflict(ApiResponse<string>
                        .Error(StatusCodes.Status409Conflict, "Já existe uma promoção ativa para este jogo."));
                }

                var entidade = new JogosPromocoes
                {
                    JogoId = input.JogoId,
                    PromocaoId = input.PromocaoId,
                    Desconto = input.Desconto,
                    UsuarioId = usuario.Id,
                };

                _jogosPromocoesRepository.Cadastrar(entidade);

                _logger.LogInformation("Promoção adicionada ao jogo. JogoId: {JogoId}, PromocaoId: {PromocaoId}", input.JogoId, input.PromocaoId);

                return Ok("Promoção cadastrada");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao criar promoção para jogo.");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>
                        .Error(StatusCodes.Status500InternalServerError, $"Erro interno: {e.Message}"));
            }
        }

        [HttpGet("Obter/{id}")]
        [ProducesResponseType(typeof(ApiResponse<JogosPromocoes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Obter(int id)
        {
            var entidade = _jogosPromocoesRepository.GetPorId(id);
            if (entidade == null)
            {
                _logger.LogWarning("Promoção do jogo não encontrada. Id: {Id}", id);
                return NotFound(ApiResponse<string>.Error(StatusCodes.Status404NotFound, "Promoção do jogo não encontrada."));
            }

            _logger.LogInformation("Promoção do jogo encontrada. Id: {Id}", id);
            return Ok(ApiResponse<JogosPromocoes>.Ok(entidade));
        }

        [HttpGet("ObterTodos")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<JogosPromocoes>>), StatusCodes.Status200OK)]
        public IActionResult ObterTodos()
        {
            var entidades = _jogosPromocoesRepository.GetTodos();

            _logger.LogInformation("Listagem de promoções dos jogos realizada. Total: {Total}", entidades.Count());
            return Ok(ApiResponse<IEnumerable<JogosPromocoes>>.Ok(entidades));
        }

        [HttpDelete("Deletar/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Deletar(int id)
        {
            var entidade = _jogosPromocoesRepository.GetPorId(id);
            if (entidade == null)
            {
                _logger.LogWarning("Promoção do jogo não encontrada para deletar. Id: {Id}", id);
                return NotFound(ApiResponse<string>.Error(StatusCodes.Status404NotFound, "Promoção do jogo não encontrada."));
            }

            _jogosPromocoesRepository.Deletar(id);

            _logger.LogInformation("Promoção do jogo deletada. Id: {Id}", id);
            return Ok(ApiResponse<string>.Ok("Promoção excluída com sucesso."));
        }
    }
}
