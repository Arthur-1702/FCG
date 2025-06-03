using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using FiapCloudGames.Api.Auth;
using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repository;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UsuarioJogoPropriedadeController : Controller
    {
        private readonly IUsuarioJogoPropriedadeRepository _usuarioJogoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IJogosPromocoesRepository _jogosPromocoesRepository;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioJogoPropriedadeController(IUsuarioJogoPropriedadeRepository usuarioJogoRepository,
                                     IUsuarioRepository usuarioRepository,
                                     IJogoRepository jogoRepository,
                                     IJogosPromocoesRepository jogosPromocoes,
                                     ILogger<UsuarioController> logger)
        {
            _usuarioJogoRepository = usuarioJogoRepository;
            _usuarioRepository = usuarioRepository;
            _jogoRepository = jogoRepository;
            _jogosPromocoesRepository = jogosPromocoes;
            _logger = logger;
        }

        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status500InternalServerError)]
        [HttpPost("comprar")]
        [Authorize]
        public IActionResult Comprar([FromBody] CadastroUsuarioJogoDTO input)
        {
            try
            {
                _logger.LogInformation("Iniciando processo de compra. UsuarioId: {UsuarioId}, JogoId: {JogoId}, PromocaoId: {PromocaoId}",
                    input.UsuarioId, input.JogoId, input.PromocaoId);

                var usuario = UserInfo.GetUsuarioLogado(User);
                if (usuario == null || usuario.Id != input.UsuarioId)
                {
                    _logger.LogError(
                        "Usuário não autorizado para compra. UsuarioId informado: {UsuarioIdInformado}, UsuarioLogado: {UsuarioLogado}",
                        input.UsuarioId,
                        usuario?.Id.ToString() ?? "Nenhum"
                    );

                    return StatusCode(StatusCodes.Status401Unauthorized,
                        ApiResponse<CompraResponse>.Error(StatusCodes.Status401Unauthorized, "Usuário não autorizado para compra."));
                }

                var jogo = _jogoRepository.GetPorId(input.JogoId);
                if (jogo == null)
                {
                    _logger.LogWarning("Tentativa de compra para JogoId {JogoId} que não existe.", input.JogoId);

                    return StatusCode(StatusCodes.Status404NotFound,
                        ApiResponse<CompraResponse>.Error(StatusCodes.Status404NotFound, "Jogo não encontrado."));
                }

                var jaPossui = _usuarioJogoRepository.GetPorIdUsuarioIdJogo(input.UsuarioId, input.JogoId);
                if (jaPossui != null)
                {
                    _logger.LogWarning("Usuário {UsuarioId} tentou comprar o JogoId {JogoId} que já possui.", input.UsuarioId, input.JogoId);

                    return StatusCode(StatusCodes.Status409Conflict,
                        ApiResponse<CompraResponse>.Error(StatusCodes.Status409Conflict, "Usuário já possui este jogo."));
                }

                var promocao = _jogosPromocoesRepository.GetPromocaoAtiva(input.JogoId, input.PromocaoId);
                var descontoAplicado = promocao?.Desconto ?? 0;
                var precoFinal = (jogo.Preco * (1 - descontoAplicado / 100));

                _logger.LogInformation("Preço final calculado para JogoId {JogoId}: {PrecoFinal} (Desconto aplicado: {Desconto})",
                    input.JogoId, precoFinal, descontoAplicado);

                var saldoAtual = _usuarioRepository.ConferirSaldo(input.UsuarioId);
                _logger.LogInformation("Saldo atual do usuário {UsuarioId}: {SaldoAtual}", input.UsuarioId, saldoAtual);

                if (saldoAtual < precoFinal)
                {
                    _logger.LogWarning("Saldo insuficiente. UsuarioId: {UsuarioId}, Saldo: {SaldoAtual}, PrecoFinal: {PrecoFinal}",
                        input.UsuarioId, saldoAtual, precoFinal);

                    return StatusCode(StatusCodes.Status422UnprocessableEntity,
                        ApiResponse<CompraResponse>.Error(StatusCodes.Status422UnprocessableEntity, "Saldo insuficiente para comprar o jogo."));
                }

                _usuarioRepository.Subtrair(input.UsuarioId, precoFinal);
                _logger.LogInformation("Saldo subtraído com sucesso. UsuarioId: {UsuarioId}, Valor: {PrecoFinal}", input.UsuarioId, precoFinal);

                var usuarioJogo = new UsuarioJogoPropriedade
                {
                    UsuarioId = input.UsuarioId,
                    JogoId = input.JogoId,
                    ValorPago = precoFinal,
                    PromocaoId = promocao?.PromocaoId
                };
                _usuarioJogoRepository.Cadastrar(usuarioJogo);

                _logger.LogInformation("Compra cadastrada com sucesso. UsuarioId: {UsuarioId}, JogoId: {JogoId}, PromocaoId: {PromocaoId}",
                    input.UsuarioId, input.JogoId, promocao?.PromocaoId.ToString() ?? "Nenhum");

                var response = new CompraResponse(
                    input.UsuarioId,
                    input.JogoId,
                    jogo.Nome,
                    precoFinal,
                    promocao?.PromocaoId,
                    descontoAplicado,
                    "Compra realizada com sucesso."
                );

                _logger.LogInformation("Compra finalizada com sucesso para UsuarioId: {UsuarioId}, JogoId: {JogoId}", input.UsuarioId, input.JogoId);

                return Ok(ApiResponse<CompraResponse>.Ok(response));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Erro inesperado no processo de compra. UsuarioId: {UsuarioId}, JogoId: {JogoId}", input.UsuarioId, input.JogoId);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CompraResponse>.Error(StatusCodes.Status500InternalServerError, $"Erro interno: {e.Message}"));
            }
        }

        // Trazer jogos deste usuário
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<CompraResponse>), StatusCodes.Status500InternalServerError)]
        [HttpGet("{usuarioId:int}")]
        public IActionResult Get([FromRoute] int usuarioId)
        {
            try
            {
                _logger.LogInformation("Iniciando processo de obtenção de jogos do usuário, para o usuário id {UsuarioId}", usuarioId);

                var response = _usuarioJogoRepository.GetJogosCompradosPorUsuario(usuarioId);

                _logger.LogInformation("Sucesso na obtenção de jogos do usuário, para o usuário id {UsuarioId}", usuarioId);

                return Ok(ApiResponse<List<UsuarioJogoPropriedade>>.Ok(response));
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Erro inesperado no processo de Get jogos do usuários. UsuarioId: {UsuarioId}", usuarioId);

                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<UsuarioJogoPropriedade>.Error(StatusCodes.Status500InternalServerError, $"Erro interno: {e.Message}"));
            }
        }
    }
}
