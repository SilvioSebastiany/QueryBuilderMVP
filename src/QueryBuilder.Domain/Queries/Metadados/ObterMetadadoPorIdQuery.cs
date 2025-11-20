using MediatR;
using QueryBuilder.Domain.Entities;

namespace QueryBuilder.Domain.Queries.Metadados;

/// <summary>
/// Query para obter metadado por ID
/// </summary>
public record ObterMetadadoPorIdQuery(
    int Id
) : IRequest<TabelaDinamica?>;
