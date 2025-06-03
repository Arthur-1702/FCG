using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Responses;
using FiapCloudGames.Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repository;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromocaoController : ControllerBase
    {
        private readonly IPromocaoRepository _promocaoRepository;
        private readonly ILogger<PromocaoController> _logger;

        public PromocaoController(IPromocaoRepository promocaoRepository, ILogger<PromocaoController> logger)
        {
            _promocaoRepository = promocaoRepository;
            _logger = logger;
        }

        [HttpPost("Cadastrar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult Cadastrar([FromBody] PromocaoDTO input)
        {
            try
            {
                var usuarioLogado = UserInfo.GetUsuarioLogado(User);
                if (usuarioLogado == null)
                {
                    _logger.LogWarning("Tentativa de criar promoção sem usuário autenticado.");
                    return Unauthorized(ApiResponse<string>.Error(StatusCodes.Status401Unauthorized, "Usuário não autorizado"));
                }

                var dataFimMenorOuIgualADataInicio = input.DataFim <= input.DataInicio;                
                if (dataFimMenorOuIgualADataInicio)
                {
                    _logger.LogWarning($"DataFim não pode ser menor ou igual a DataInicio." +
                        $" DataInicio: {input.DataInicio}, DataFim: {input.DataFim}", input.DataInicio, input.DataFim);
                    return BadRequest(ApiResponse<string>
                        .Error(StatusCodes.Status400BadRequest, "Data final deve ser maior que a data inicial."));
                }

                var existePromocaoComNome = _promocaoRepository.TemPromocaoComNome(input.Descricao);
                if (existePromocaoComNome)
                {
                    _logger.LogWarning($"Já existe uma promoção com o nome {input.Descricao}", input.Descricao);
                    return Conflict(ApiResponse<string>
                        .Error(StatusCodes.Status409Conflict, "Já existe uma promoção com esta descrição."));
                }

                var promocao = new Promocao
                {
                    Descricao = input.Descricao,
                    DataInicio = input.DataInicio,
                    DataFim = input.DataFim,
                    Ativo = input.Ativo,
                    UsuarioId = usuarioLogado.Id

                };

                _promocaoRepository.Cadastrar(promocao);
                _logger.LogInformation("Promoção criada com sucesso. PromoçãoId: {PromocaoId}, CriadaPor: {UsuarioId}",
                    promocao.Id, usuarioLogado.Id);

                return StatusCode(StatusCodes.Status201Created, ApiResponse<int>.Ok(promocao.Id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao criar promoção");
                return StatusCode(500, ApiResponse<string>.Error(StatusCodes.Status500InternalServerError, $"Erro interno: {e.Message}"));
            }
        }

        [HttpGet("Obter/{id}")]
        [ProducesResponseType(typeof(ApiResponse<Promocao>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Obter(int id)
        {
            var promocao = _promocaoRepository.GetPorId(id);
            if (promocao == null)
            {
                _logger.LogWarning("Promoção não encontrada. PromoçãoId: {PromocaoId}", id);
                return NotFound(ApiResponse<string>.Error(404, "Promoção não encontrada"));
            }

            _logger.LogInformation("Promoção consultada. PromoçãoId: {PromocaoId}", id);
            return Ok(ApiResponse<Promocao>.Ok(promocao));
        }

        [HttpGet("GetTodos")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Promocao>>), StatusCodes.Status200OK)]
        public IActionResult GetTodos()
        {
            var promocoes = _promocaoRepository.GetTodos();
            _logger.LogInformation("Listagem de promoções realizada. Total: {Total}", promocoes.Count());
            return Ok(ApiResponse<IEnumerable<Promocao>>.Ok(promocoes));
        }

        [HttpPut("Alterar/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<Promocao>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Alterar(int id, [FromBody] PromocaoDTO input)
        {
            var promocao = _promocaoRepository.GetPorId(id);
            if (promocao == null)
            {
                _logger.LogWarning("Tentativa de atualizar promoção inexistente. PromoçãoId: {PromocaoId}", id);
                return NotFound(ApiResponse<string>.Error(StatusCodes.Status404NotFound, "Promoção não encontrada"));
            }

            promocao.Descricao = input.Descricao;
            promocao.DataInicio = input.DataInicio;
            promocao.DataFim = input.DataFim;
            promocao.Ativo = input.Ativo;
            promocao.UsuarioId = UserInfo.GetUsuarioLogado(User)!.Id;
            _promocaoRepository.Atualizar(promocao);

            _logger.LogInformation("Promoção atualizada com sucesso. PromoçãoId: {PromocaoId}", id);

            return Ok(ApiResponse<Promocao>.Ok(promocao));
        }

        [HttpDelete("Deletar/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult Deletar(int id)
        {
            var promocao = _promocaoRepository.GetPorId(id);
            if (promocao == null)
            {
                _logger.LogWarning("Tentativa de deletar promoção inexistente. PromoçãoId: {PromocaoId}", id);
                return NotFound(ApiResponse<string>.Error(StatusCodes.Status404NotFound, "Promoção não encontrada"));
            }

            _promocaoRepository.Deletar(id);

            _logger.LogInformation("Promoção deletada com sucesso pelo usuário {usuario}. PromoçãoId: {PromocaoId}", 
                UserInfo.GetUsuarioLogado(User)!.Nome , 
                id);

            return NoContent();
        }
    }
}
