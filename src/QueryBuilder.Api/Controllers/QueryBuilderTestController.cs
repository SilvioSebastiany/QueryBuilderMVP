using Microsoft.AspNetCore.Mvc;
using QueryBuilder.Domain.Interfaces;
using SqlKata.Compilers;

namespace QueryBuilder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryBuilderTestController : ControllerBase
{
    private readonly IQueryBuilderService _queryBuilderService;
    private readonly OracleCompiler _compiler;
    private readonly ILogger<QueryBuilderTestController> _logger;

    public QueryBuilderTestController(
        IQueryBuilderService queryBuilderService,
        OracleCompiler compiler,
        ILogger<QueryBuilderTestController> logger)
    {
        _queryBuilderService = queryBuilderService;
        _compiler = compiler;
        _logger = logger;
    }

    /// <summary>
    /// Testa geração de query simples (sem JOINs)
    /// </summary>
    /// <param name="tabela">Nome da tabela</param>
    [HttpGet("simples/{tabela}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GerarQuerySimples(string tabela)
    {
        try
        {
            _logger.LogInformation("Testando query simples para tabela: {Tabela}", tabela);

            var query = _queryBuilderService.MontarQuery(tabela, incluirJoins: false);
            var compiled = _compiler.Compile(query);

            return Ok(new
            {
                Tabela = tabela,
                IncluiJoins = false,
                SQL = compiled.Sql,
                Parametros = compiled.NamedBindings,
                Sucesso = true
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Tabela não encontrada: {Tabela}", tabela);
            return NotFound(new { Erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar query para tabela: {Tabela}", tabela);
            return BadRequest(new { Erro = ex.Message });
        }
    }

    /// <summary>
    /// Testa geração de query com JOINs
    /// </summary>
    /// <param name="tabela">Nome da tabela</param>
    /// <param name="profundidade">Profundidade máxima de JOINs (padrão: 2)</param>
    [HttpGet("com-joins/{tabela}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GerarQueryComJoins(string tabela, [FromQuery] int profundidade = 2)
    {
        try
        {
            _logger.LogInformation(
                "Testando query com JOINs para tabela: {Tabela}, Profundidade: {Profundidade}",
                tabela, profundidade);

            var query = _queryBuilderService.MontarQuery(
                tabela,
                incluirJoins: true,
                profundidadeMaxima: profundidade);

            var compiled = _compiler.Compile(query);

            return Ok(new
            {
                Tabela = tabela,
                IncluiJoins = true,
                ProfundidadeMaxima = profundidade,
                SQL = compiled.Sql,
                Parametros = compiled.NamedBindings,
                Sucesso = true
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Tabela não encontrada: {Tabela}", tabela);
            return NotFound(new { Erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar query para tabela: {Tabela}", tabela);
            return BadRequest(new { Erro = ex.Message });
        }
    }

    /// <summary>
    /// Testa geração de query com filtros
    /// </summary>
    /// <param name="tabela">Nome da tabela</param>
    [HttpPost("com-filtros/{tabela}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GerarQueryComFiltros(
        string tabela,
        [FromBody] Dictionary<string, object> filtros)
    {
        try
        {
            _logger.LogInformation(
                "Testando query com filtros para tabela: {Tabela}, Total filtros: {TotalFiltros}",
                tabela, filtros.Count);

            var query = _queryBuilderService.MontarQueryComFiltros(
                tabela,
                filtros,
                incluirJoins: false);

            var compiled = _compiler.Compile(query);

            return Ok(new
            {
                Tabela = tabela,
                TotalFiltros = filtros.Count,
                SQL = compiled.Sql,
                Parametros = compiled.NamedBindings,
                Sucesso = true
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Tabela não encontrada: {Tabela}", tabela);
            return NotFound(new { Erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar query para tabela: {Tabela}", tabela);
            return BadRequest(new { Erro = ex.Message });
        }
    }

    /// <summary>
    /// Lista todas as tabelas disponíveis nos metadados
    /// </summary>
    [HttpGet("tabelas-disponiveis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListarTabelasDisponiveis()
    {
        try
        {
            var tabelas = _queryBuilderService.ListarTabelas();

            return Ok(new
            {
                TotalTabelas = tabelas.Count,
                Tabelas = tabelas,
                Sucesso = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tabelas disponíveis");
            return BadRequest(new { Erro = ex.Message });
        }
    }

    /// <summary>
    /// Lista tabelas conhecidas hardcoded para referência
    /// </summary>
    [HttpGet("tabelas-conhecidas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListarTabelasConhecidas()
    {
        try
        {
            var tabelasConhecidas = new[]
            {
                "CLIENTES",
                "PEDIDOS",
                "PRODUTOS",
                "CATEGORIAS",
                "ITENS_PEDIDO",
                "ENDERECOS"
            };

            return Ok(new
            {
                TotalTabelas = tabelasConhecidas.Length,
                Tabelas = tabelasConhecidas,
                Sucesso = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar tabelas disponíveis");
            return BadRequest(new { Erro = ex.Message });
        }
    }
}
