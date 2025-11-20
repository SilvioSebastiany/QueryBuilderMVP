using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueryBuilder.Domain.Entities;
using QueryBuilder.Domain.Interfaces;
using QueryBuilder.Domain.Notifications;
using QueryBuilder.Domain.Queries.Metadados;

namespace QueryBuilder.Api.Controllers
{
    /// <summary>
    /// Controller de Metadados - CQRS + MediatR Pattern
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MetadadosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly INotificationContext _notificationContext;
        private readonly ILogger<MetadadosController> _logger;

        public MetadadosController(
            IMediator mediator,
            INotificationContext notificationContext,
            ILogger<MetadadosController> logger)
        {
            _mediator = mediator;
            _notificationContext = notificationContext;
            _logger = logger;
        }

        /// <summary>
        /// Rota de teste para verificar se a API está funcionando
        /// </summary>
        [HttpGet("teste")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Teste()
        {
            _logger.LogInformation("Rota de teste acessada com sucesso!");

            return Ok(new
            {
                Mensagem = "API QueryBuilder está funcionando! 🚀",
                Versao = "1.0.0",
                Timestamp = DateTime.Now,
                Endpoints = new[]
                {
                    "GET /api/metadados/teste - Esta rota de teste",
                    "GET /api/metadados - Listar todos os metadados",
                    "GET /api/metadados/{id} - Obter metadado por ID",
                    "GET /api/metadados/tabela/{nome} - Obter metadado por nome da tabela"
                }
            });
        }

        /// <summary>
        /// Lista todos os metadados cadastrados (CQRS Pattern)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos([FromQuery] bool apenasAtivos = true)
        {
            var query = new ObterTodosMetadadosQuery(apenasAtivos);
            var resultado = await _mediator.Send(query);

            if (_notificationContext.HasNotifications)
            {
                return BadRequest(new
                {
                    Erros = _notificationContext.Notifications.Select(n => new
                    {
                        Campo = n.Key,
                        Mensagem = n.Message
                    })
                });
            }

            if (resultado == null)
            {
                return StatusCode(500, new { Erro = "Erro ao obter metadados" });
            }

            return Ok(new
            {
                Total = resultado.Total,
                Dados = resultado.Metadados.Select(m => new
                {
                    m.Id,
                    m.Tabela,
                    m.CamposDisponiveis,
                    m.ChavePk,
                    m.VinculoEntreTabela,
                    m.DescricaoTabela,
                    m.VisivelParaIA,
                    m.Ativo,
                    m.DataCriacao
                })
            });
        }

        /// <summary>
        /// Obtém um metadado por ID (CQRS Pattern)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var query = new ObterMetadadoPorIdQuery(id);
            var metadado = await _mediator.Send(query);

            if (_notificationContext.HasNotifications)
            {
                var notificacao = _notificationContext.Notifications.FirstOrDefault();
                if (notificacao?.Key == "NotFound")
                {
                    return NotFound(new { Mensagem = notificacao.Message });
                }

                return BadRequest(new
                {
                    Erros = _notificationContext.Notifications.Select(n => new
                    {
                        Campo = n.Key,
                        Mensagem = n.Message
                    })
                });
            }

            if (metadado == null)
            {
                return NotFound(new { Mensagem = $"Metadado com ID {id} não encontrado" });
            }

            return Ok(metadado);
        }

        /// <summary>
        /// Obtém um metadado pelo nome da tabela (CQRS Pattern)
        /// </summary>
        [HttpGet("tabela/{nome}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPorNomeTabela(string nome)
        {
            var query = new ObterMetadadoPorTabelaQuery(nome);
            var metadado = await _mediator.Send(query);

            if (_notificationContext.HasNotifications)
            {
                var notificacao = _notificationContext.Notifications.FirstOrDefault();
                if (notificacao?.Key == "NotFound")
                {
                    return NotFound(new { Mensagem = notificacao.Message });
                }

                return BadRequest(new
                {
                    Erros = _notificationContext.Notifications.Select(n => new
                    {
                        Campo = n.Key,
                        Mensagem = n.Message
                    })
                });
            }

            if (metadado == null)
            {
                return NotFound(new { Mensagem = $"Metadado para tabela '{nome}' não encontrado" });
            }

            return Ok(metadado);
        }
    }
}
