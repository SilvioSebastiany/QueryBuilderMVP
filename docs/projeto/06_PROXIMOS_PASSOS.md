# ⏭️ Próximos Passos

## 🎯 Prioridades Imediatas (Esta Semana)

### 1. 🔴 PRIORIDADE MÁXIMA: QueryBuilderService

**Por que é prioritário:**
- É o coração do sistema
- Sem ele, não há geração dinâmica de queries
- Bloqueia todos os outros desenvolvimentos

**Tempo estimado:** 5-7 dias
**Complexidade:** ⭐⭐⭐⭐

#### Checklist de Implementação

**Dia 1-2: Estrutura Básica**
- [ ] Criar arquivo `src/QueryBuilder.Domain/Services/QueryBuilderService.cs`
- [ ] Implementar interface `IQueryBuilderService`
- [ ] Injetar `IMetadadosRepository` no construtor
- [ ] Criar método base `MontarQueryAsync(string nomeTabela)`

**Código inicial:**
```csharp
public class QueryBuilderService : IQueryBuilderService
{
    private readonly IMetadadosRepository _metadadosRepository;
    private readonly OracleCompiler _compiler;

    public QueryBuilderService(IMetadadosRepository metadadosRepository)
    {
        _metadadosRepository = metadadosRepository;
        _compiler = new OracleCompiler();
    }

    public async Task<Query> MontarQueryAsync(string nomeTabela, bool incluirJoins = false)
    {
        // TODO: Implementar
    }
}
```

**Dia 3-4: Lógica de Geração de Queries**
- [ ] Buscar metadados da tabela
- [ ] Parsear campos disponíveis
- [ ] Criar query base com SELECT
- [ ] Implementar lógica de JOINs se `incluirJoins = true`
- [ ] Parsear vínculos entre tabelas

**Lógica de parsing de vínculos:**
```csharp
private List<(string TabelaDestino, string CampoFK, string CampoPK)> ParseVinculos(string vinculo)
{
    // Formato: "PEDIDOS:ID_CLIENTE:ID;ENDERECOS:ID_CLIENTE:ID"
    var vinculos = new List<(string, string, string)>();

    if (string.IsNullOrWhiteSpace(vinculo))
        return vinculos;

    foreach (var v in vinculo.Split(';'))
    {
        var partes = v.Split(':');
        if (partes.Length == 3)
        {
            vinculos.Add((partes[0].Trim(), partes[1].Trim(), partes[2].Trim()));
        }
    }

    return vinculos;
}
```

**Dia 5: JOINs Recursivos**
- [ ] Implementar profundidade de JOINs
- [ ] Prevenção de loops infinitos
- [ ] HashSet de tabelas já processadas
- [ ] Limite de profundidade configurável

**Dia 6-7: Testes e Refinamento**
- [ ] Criar testes unitários
- [ ] Testar com dados reais
- [ ] Validar SQL gerado
- [ ] Documentar uso

---

### 2. 🟡 ConsultaDinamicaRepository

**Tempo estimado:** 2-3 dias
**Complexidade:** ⭐⭐⭐

#### Checklist
- [ ] Criar `src/QueryBuilder.Infra.Data/Repositories/ConsultaDinamicaRepository.cs`
- [ ] Implementar `ExecutarQueryAsync(Query query)`
- [ ] Mapeamento dinâmico com Dapper
- [ ] Tratamento de timeout
- [ ] Tratamento de erros Oracle
- [ ] Logging de queries executadas

**Código base:**
```csharp
public class ConsultaDinamicaRepository : IConsultaDinamicaRepository
{
    private readonly IDbConnection _connection;
    private readonly OracleCompiler _compiler;
    private readonly ILogger<ConsultaDinamicaRepository> _logger;

    public async Task<IEnumerable<dynamic>> ExecutarQueryAsync(Query query)
    {
        var compiled = _compiler.Compile(query);

        _logger.LogInformation("Executando query: {Sql}", compiled.Sql);

        try
        {
            return await _connection.QueryAsync<dynamic>(
                compiled.Sql,
                compiled.NamedBindings,
                commandTimeout: 30
            );
        }
        catch (OracleException ex)
        {
            _logger.LogError(ex, "Erro ao executar query");
            throw;
        }
    }
}
```

---

### 3. 🟡 ConsultaDinamicaController

**Tempo estimado:** 2 dias
**Complexidade:** ⭐⭐

#### Checklist
- [ ] Criar `src/QueryBuilder.Api/Controllers/ConsultaDinamicaController.cs`
- [ ] Endpoint GET `/api/consulta/{tabela}`
- [ ] Validação de nome de tabela (WhiteList)
- [ ] Injetar QueryBuilderService
- [ ] Injetar ConsultaDinamicaRepository
- [ ] Tratamento de erros
- [ ] Documentação Swagger

**Código base:**
```csharp
[ApiController]
[Route("api/consulta")]
public class ConsultaDinamicaController : ControllerBase
{
    private readonly IQueryBuilderService _queryBuilder;
    private readonly IConsultaDinamicaRepository _repository;
    private readonly ILogger<ConsultaDinamicaController> _logger;

    [HttpGet("{tabela}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarTabela(
        string tabela,
        [FromQuery] bool incluirJoins = false)
    {
        try
        {
            // Validar tabela permitida
            if (!TabelaPermitida(tabela))
                return BadRequest(new { Erro = "Tabela não autorizada" });

            // Gerar query
            var query = await _queryBuilder.MontarQueryAsync(tabela, incluirJoins);

            // Executar
            var resultados = await _repository.ExecutarQueryAsync(query);

            return Ok(new
            {
                Tabela = tabela,
                Total = resultados.Count(),
                Dados = resultados
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar tabela {Tabela}", tabela);
            return StatusCode(500, new { Erro = "Erro ao executar consulta" });
        }
    }

    private bool TabelaPermitida(string tabela)
    {
        var permitidas = new[] { "CLIENTES", "PEDIDOS", "PRODUTOS", "CATEGORIAS", "ITENS_PEDIDO", "ENDERECOS" };
        return permitidas.Contains(tabela.ToUpper());
    }
}
```

---

### 4. 🟢 Registrar no DI Container

**Tempo estimado:** 30 minutos
**Complexidade:** ⭐

#### Checklist
- [ ] Abrir `src/QueryBuilder.Infra.CrossCutting.IoC/DependencyInjection.cs`
- [ ] Registrar `IQueryBuilderService` → `QueryBuilderService`
- [ ] Registrar `IConsultaDinamicaRepository` → `ConsultaDinamicaRepository`
- [ ] Registrar `OracleCompiler` como Singleton

**Código:**
```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // ... código existente ...

    // Domain Services
    services.AddScoped<IQueryBuilderService, QueryBuilderService>();

    // Repositories
    services.AddScoped<IMetadadosRepository, MetadadosRepository>();
    services.AddScoped<IConsultaDinamicaRepository, ConsultaDinamicaRepository>();

    // SqlKata
    services.AddSingleton<OracleCompiler>();

    return services;
}
```

---

## 📅 Cronograma Detalhado

### Semana 1 (13/11 - 19/11)
```
Seg: [█████░░░░░] QueryBuilderService - Estrutura básica
Ter: [█████████░] QueryBuilderService - Geração de queries
Qua: [██████████] QueryBuilderService - JOINs recursivos
Qui: [█████░░░░░] ConsultaDinamicaRepository - Implementação
Sex: [██████████] ConsultaDinamicaRepository - Testes
```

### Semana 2 (20/11 - 26/11)
```
Seg: [█████░░░░░] ConsultaDinamicaController - Endpoint básico
Ter: [██████████] ConsultaDinamicaController - Validações
Qua: [█████░░░░░] Testes end-to-end
Qui: [█████████░] Filtros dinâmicos - Implementação
Sex: [██████████] Documentação e refinamento
```

---

## 🧪 Como Testar Cada Componente

### Teste 1: QueryBuilderService (Isolado)

```csharp
// Criar teste unitário
[Fact]
public async Task MontarQuery_DeveGerarQueryComJoins()
{
    // Arrange
    var mockRepo = new Mock<IMetadadosRepository>();
    mockRepo.Setup(r => r.ObterPorNomeTabelaAsync("CLIENTES"))
        .ReturnsAsync(new TabelaDinamica { /* ... */ });

    var service = new QueryBuilderService(mockRepo.Object);

    // Act
    var query = await service.MontarQueryAsync("CLIENTES", incluirJoins: true);
    var compiler = new OracleCompiler();
    var sql = compiler.Compile(query);

    // Assert
    Assert.Contains("JOIN", sql.Sql);
    Assert.Contains("PEDIDOS", sql.Sql);
}
```

### Teste 2: Endpoint Completo (Integração)

```http
### Teste básico
GET http://localhost:5249/api/consulta/CLIENTES
Content-Type: application/json

### Com JOINs
GET http://localhost:5249/api/consulta/CLIENTES?incluirJoins=true
Content-Type: application/json

### Validar SQL gerado (adicionar endpoint debug)
GET http://localhost:5249/api/consulta/CLIENTES/debug?incluirJoins=true
Content-Type: application/json
```

---

## 📝 Checklist de Validação

Antes de considerar a tarefa completa:

### QueryBuilderService ✅
- [ ] Gera query simples (sem JOINs)
- [ ] Gera query com JOINs de 1 nível
- [ ] Gera query com JOINs de 2+ níveis
- [ ] Previne loops infinitos
- [ ] Respeita limite de profundidade
- [ ] Lida com tabelas sem vínculos
- [ ] Lida com vínculos malformados
- [ ] SQL gerado é válido
- [ ] Testes unitários passando

### ConsultaDinamicaRepository ✅
- [ ] Executa query simples
- [ ] Executa query com JOINs
- [ ] Retorna resultados corretos
- [ ] Lida com timeout
- [ ] Lida com erros Oracle
- [ ] Log de queries funciona
- [ ] Parâmetros são sanitizados

### ConsultaDinamicaController ✅
- [ ] Endpoint responde 200
- [ ] Valida tabela permitida
- [ ] Retorna 404 para tabela inexistente
- [ ] Retorna 400 para tabela não autorizada
- [ ] JSON de resposta correto
- [ ] Swagger documentado
- [ ] Tratamento de erros funciona

---

## 🎯 Definição de Pronto (DoD)

Uma tarefa só está completa quando:

✅ Código implementado
✅ Testes unitários criados e passando
✅ Testes de integração funcionando
✅ Código revisado (self-review)
✅ Sem warnings de compilação
✅ Documentação atualizada
✅ Swagger atualizado (se API)
✅ Commit com mensagem clara
✅ Funcionalidade testada manualmente

---

## 🚨 Riscos e Mitigações

### Risco 1: JOINs Recursivos Complexos
**Probabilidade:** Alta
**Impacto:** Alto
**Mitigação:**
- Implementar limite de profundidade
- HashSet de tabelas visitadas
- Testes extensivos com grafos de relacionamentos

### Risco 2: Performance de Queries
**Probabilidade:** Média
**Impacto:** Alto
**Mitigação:**
- Timeout configurável
- Logging de tempo de execução
- Cache de metadados
- Índices no banco

### Risco 3: SQL Injection
**Probabilidade:** Baixa
**Impacto:** Crítico
**Mitigação:**
- Usar SqlKata (já sanitiza)
- WhiteList de tabelas
- Validação rigorosa de entrada
- Testes de segurança

---

## 💡 Dicas de Implementação

### 1. Comece Simples
Implemente primeiro sem JOINs, depois adicione a complexidade.

### 2. Teste Incrementalmente
Não espere terminar tudo para testar. Teste cada método isoladamente.

### 3. Use TDD (Test-Driven Development)
Escreva o teste antes do código. Ajuda a pensar na interface.

### 4. Documente Conforme Desenvolve
Não deixe documentação para depois. Faça enquanto o contexto está fresco.

### 5. Commit Frequentemente
Commits pequenos e frequentes facilitam rollback se necessário.

---

## 📚 Recursos Úteis

### Documentação
- [SqlKata Documentation](https://sqlkata.com/docs)
- [Dapper GitHub](https://github.com/DapperLib/Dapper)
- [Oracle .NET Developer Center](https://www.oracle.com/database/technologies/appdev/dotnet.html)

### Referências de Código
- Ver exemplo em `docs/EXEMPLO_08_METADADOS.md`
- Estudar `MetadadosRepository.cs` existente

### Ferramentas
- **SQL Developer** - Para testar queries geradas manualmente
- **Postman/REST Client** - Para testar endpoints
- **Docker logs** - Para debug de erros Oracle

---

## 🎉 Marcos (Milestones)

### Milestone 1: Query Builder Básico ⏳
**Data alvo:** 19/11/2025
- [x] QueryBuilderService implementado
- [ ] Gera queries sem JOINs
- [ ] Testes unitários passando

### Milestone 2: Query Builder com JOINs ⏳
**Data alvo:** 22/11/2025
- [ ] JOINs de 1 nível funcionando
- [ ] JOINs recursivos funcionando
- [ ] Prevenção de loops

### Milestone 3: API Completa ⏳
**Data alvo:** 26/11/2025
- [ ] Endpoint de consulta funcionando
- [ ] Validações implementadas
- [ ] Testes end-to-end passando

### Milestone 4: MVP Funcional 🎯
**Data alvo:** 30/11/2025
- [ ] Filtros dinâmicos
- [ ] Ordenação
- [ ] Paginação
- [ ] Documentação completa

---

## 📞 Quando Pedir Ajuda

Se travar por mais de 2 horas no mesmo problema:
1. Revisar a documentação
2. Buscar exemplos similares
3. Fazer uma pausa (rubber duck debugging)
4. Perguntar em fóruns (.NET, Stack Overflow)

Lembre-se: **Travar faz parte do aprendizado!** 🧠

---

<div align="center">

**⏭️ Um passo de cada vez, mas sempre para frente! 🚀**

[← Voltar ao Índice](00_INDICE.md) | [Ver Roadmap Completo →](05_ROADMAP.md)

</div>
