using Microsoft.AspNetCore.Mvc;
using QueryBuilder.Domain.Interfaces;
using QueryBuilder.Domain.Entities;

namespace QueryBuilder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadadosController : ControllerBase
    {
        private readonly IMetadadosRepository _repository;
        private readonly ILogger<MetadadosController> _logger;

        public MetadadosController(
            IMetadadosRepository repository,
            ILogger<MetadadosController> logger)
        {
            _repository = repository;
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
                    "GET /api/metadados/tabela/{nome} - Obter metadado por nome da tabela",
                    "POST /api/metadados - Criar novo metadado"
                }
            });
        }

        /// <summary>
        /// Lista todos os metadados cadastrados
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos([FromQuery] bool apenasAtivos = true)
        {
            try
            {
                var metadados = await _repository.ObterTodosAsync(apenasAtivos);

                return Ok(new
                {
                    Total = metadados.Count(),
                    Dados = metadados.Select(m => new
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter metadados");
                return StatusCode(500, new { Erro = "Erro ao obter metadados", Detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um metadado por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var metadado = await _repository.ObterPorIdAsync(id);

                if (metadado == null)
                    return NotFound(new { Mensagem = $"Metadado com ID {id} não encontrado" });

                return Ok(metadado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter metadado por ID: {Id}", id);
                return StatusCode(500, new { Erro = "Erro ao obter metadado", Detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um metadado pelo nome da tabela
        /// </summary>
        [HttpGet("tabela/{nome}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorNomeTabela(string nome)
        {
            try
            {
                var metadado = await _repository.ObterPorNomeTabelaAsync(nome);

                if (metadado == null)
                    return NotFound(new { Mensagem = $"Metadado para tabela '{nome}' não encontrado" });

                return Ok(metadado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter metadado por nome da tabela: {Nome}", nome);
                return StatusCode(500, new { Erro = "Erro ao obter metadado", Detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo metadado
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Criar([FromBody] CriarMetadadoRequest request)
        {
            try
            {
                // Verificar se já existe
                if (await _repository.ExisteAsync(request.Tabela))
                    return BadRequest(new { Erro = $"Já existe um metadado para a tabela '{request.Tabela}'" });

                // Criar entity
                var metadado = TabelaDinamica.Criar(
                    tabela: request.Tabela,
                    camposDisponiveis: request.CamposDisponiveis,
                    chavePk: request.ChavePk,
                    vinculoEntreTabela: request.VinculoEntreTabela,
                    descricaoTabela: request.DescricaoTabela,
                    descricaoCampos: request.DescricaoCampos,
                    visivelParaIA: request.VisivelParaIA ?? true
                );

                // Salvar
                var id = await _repository.CriarAsync(metadado);

                _logger.LogInformation("Metadado criado com sucesso. ID: {Id}, Tabela: {Tabela}", id, request.Tabela);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id },
                    new { Id = id, Mensagem = "Metadado criado com sucesso" }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Erro = "Dados inválidos", Detalhes = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar metadado");
                return StatusCode(500, new { Erro = "Erro ao criar metadado", Detalhes = ex.Message });
            }
        }
    }

    // DTO para criação de metadado
    public record CriarMetadadoRequest(
        string Tabela,
        string CamposDisponiveis,
        string ChavePk,
        string? VinculoEntreTabela = null,
        string? DescricaoTabela = null,
        string? DescricaoCampos = null,
        bool? VisivelParaIA = true
    );
}
