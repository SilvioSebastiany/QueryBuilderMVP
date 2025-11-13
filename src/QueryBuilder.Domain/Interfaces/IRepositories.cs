using System;
using System.Collections.Generic;
using QueryBuilder.Domain.Entities;
using SqlKata;

namespace QueryBuilder.Domain.Interfaces
{
    /// <summary>
    /// Interface do repositório de metadados
    /// </summary>
    public interface IMetadadosRepository
    {
        Task<TabelaDinamica?> ObterPorIdAsync(int id);
        Task<TabelaDinamica?> ObterPorNomeTabelaAsync(string nomeTabela);
        Task<IEnumerable<TabelaDinamica>> ObterTodosAsync(bool apenasAtivos = true);
        Task<IEnumerable<TabelaDinamica>> ObterVisiveisParaIAAsync();
        Task<int> CriarAsync(TabelaDinamica tabela);
        Task AtualizarAsync(TabelaDinamica tabela);
        Task DeletarAsync(int id);
        Task<bool> ExisteAsync(string nomeTabela);
        Task<IEnumerable<TabelaDinamica>> ObterPorVinculoAsync(string nomeTabela);
    }

    /// <summary>
    /// Interface do serviço de montagem de queries dinâmicas
    /// </summary>
    public interface IQueryBuilderService
    {
        Task CarregarMetadadosAsync();
        Query MontarQuery(string tabelaBase, bool incluirJoins = true, int profundidadeMaxima = 3);
        Query MontarQueryComFiltros(string tabelaBase, Dictionary<string, object> filtros, bool incluirJoins = true);
        Query MontarQueryComOrdenacao(string tabelaBase, string campoOrdenacao, bool descendente = false, bool incluirJoins = true);
        Query MontarQueryComPaginacao(string tabelaBase, int pagina, int itensPorPagina, bool incluirJoins = true);
        SqlResult CompilarQuery(Query query);
        List<string> ListarTabelas();
        bool TabelaExiste(string tabela);
        string ObterGrafoRelacionamentos(string tabelaBase, int profundidade = 3);
    }

    /// <summary>
    /// Interface do serviço de catálogo para IA
    /// </summary>
    public interface IIADataCatalogService
    {
        Task<string> GerarPromptParaIAAsync();
        Task<string> GerarContextoTabelaAsync(string nomeTabela);
        Task<Dictionary<string, string>> ObterDicionarioTabelasAsync();
        Task<bool> ValidarQueryIAAsync(string query);
    }

    /// <summary>
    /// Interface do serviço de validação de metadados
    /// </summary>
    public interface IValidacaoMetadadosService
    {
        Task<List<string>> ValidarMetadadoAsync(TabelaDinamica metadado);
        Task<bool> CampoExisteNoBancoAsync(string tabela, string campo);
        Task<bool> TabelaExisteNoBancoAsync(string tabela);
        Task<List<string>> ValidarVinculosAsync(TabelaDinamica metadado);
    }

    /// <summary>
    /// Interface do repositório de consultas dinâmicas
    /// </summary>
    public interface IConsultaDinamicaRepository
    {
        Task<IEnumerable<dynamic>> ExecutarQueryAsync(Query query);
        Task<int> ExecutarQueryCountAsync(Query query);
        Task<T?> ExecutarQuerySingleAsync<T>(Query query);
        Task<IEnumerable<T>> ExecutarQueryAsync<T>(Query query);
    }
}
