using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.DTOs;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Responses;
using FiapCloudGames.Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class JogoController : ControllerBase
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly ILogger<JogoController> _logger;

        public JogoController(IJogoRepository jogoRepository,
                              ILogger<JogoController> logger)
        {
            _jogoRepository = jogoRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string? filtroNome)
        {
            try
            {
                var jogosDTO = new List<JogoDTO>();
                List<Jogo> jogos = [];
                if (filtroNome == null)
                    jogos.AddRange(_jogoRepository.GetTodos());
                else
                    jogos.AddRange(_jogoRepository.GetTodosPorFiltro(filtroNome));

                foreach (var jogo in jogos)
                {
                    jogosDTO.Add(new JogoDTO()
                    {
                        Id = jogo.Id,
                        Nome = jogo.Nome,
                        Genero = jogo.Genero,
                        Descricao = jogo.Descricao,
                        Preco = jogo.Preco,                        
                        DataCriacao = jogo.DataCriacao
                    });
                }

                return Ok(jogosDTO);
            }
            catch (Exception ex)
            {
                string mensagem = "Erro ao consultar jogos.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                Jogo jogo = _jogoRepository.GetPorId(id);
                JogoResponse response = new
                (
                    jogo.Nome,
                    jogo.Descricao,
                    jogo.Preco,
                    jogo.Genero
                );

                return Ok(ApiResponse<JogoResponse>.Ok(response));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao consultar jogo.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        [HttpPost("Cadastrar")]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] JogoDTO input)
        {
            try
            {
                var jogo = new Jogo()
                {
                    Nome = input.Nome?.Trim() ?? string.Empty,
                    Genero = input.Genero,
                    Descricao = input.Descricao?.Trim() ?? string.Empty,
                    Preco = input.Preco,
                    DataCriacao = DateTime.Now,                    
                    UsuarioId = UserInfo.GetUsuarioLogado(User)!.Id
                };

                CheckJogo(jogo.Nome);

                _jogoRepository.Cadastrar(jogo);

                _logger.LogInformation($"Jogo cadastrado com sucesso! {jogo.Nome} ");
                return Ok(jogo);
            }
            catch (Exception ex) when ((ex.Message ?? string.Empty).Contains("Jogo já cadastrado"))
            {
                string mensagem = $"Jogo: {input.Nome}, já está cadastrado.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(mensagem);
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao tentar cadastrar: {input.Nome}.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(StatusCodes.Status400BadRequest, mensagem));
            }
        }

        private void CheckJogo(string nome)
        {
            var jogo = _jogoRepository.CheckJogo(nome);

            if (jogo is not null)
                throw new Exception("Jogo já cadastrado");
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Put([FromBody] AtualizarJogoDTO input)
        {
            try
            {
                Jogo jogo = _jogoRepository.GetPorId(input.Id);
                jogo.Nome = input.Nome.Trim();
                jogo.Genero = input.Genero;
                jogo.Descricao = input.Descricao.Trim();
                jogo.Preco = input.Preco;
                jogo.UsuarioId = UserInfo.GetUsuarioLogado(User)!.Id;

                _jogoRepository.Atualizar(jogo);

                string mensagem = $"Jogo {jogo.Nome} atualizado com sucesso!";
                _logger.LogInformation(mensagem);
                return Ok(ApiResponse<string>.Ok(mensagem));
            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao tentar atualizar:: {input.Nome}.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                _jogoRepository.Deletar(id);
                string mensagem = $"Jogo Id:{id} deletado com sucesso!";
                _logger.LogInformation(mensagem);
                return Ok(ApiResponse<string>.Ok(mensagem));

            }
            catch (Exception ex)
            {
                string mensagem = $"Erro ao tentar deletar jogo Id:{id}.";
                _logger.LogError(mensagem + " Erro: " + ex.Message);
                return BadRequest(ApiResponse<string>.Error(500, mensagem));
            }
        }        
    }
}
