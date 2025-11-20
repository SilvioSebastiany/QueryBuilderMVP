using MediatR;
using QueryBuilder.Domain.Entities;

namespace QueryBuilder.Domain.Queries.Metadados;

/// <summary>
/// Query para obter metadado por nome da tabela
/// </summary>
public record ObterMetadadoPorTabelaQuery(
    string NomeTabela
) : IRequest<TabelaDinamica?>;
