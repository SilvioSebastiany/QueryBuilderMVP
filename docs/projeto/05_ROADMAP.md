# 🗺️ Roadmap Completo

## 📋 Visão Geral das Fases

```
✅ Fase 1: Fundação              [████████████] 100%
🚧 Fase 2: Funcionalidades Core  [███░░░░░░░░░]  20%
⏳ Fase 3: Qualidade             [░░░░░░░░░░░░]   0%
⏳ Fase 4: Melhorias             [░░░░░░░░░░░░]   0%
⏳ Fase 5: IA & Automação        [░░░░░░░░░░░░]   0%
⏳ Fase 6: Multi-Database        [░░░░░░░░░░░░]   0%
```

---

## ✅ Fase 1: Fundação (COMPLETO)

**Status:** ✅ 100% Concluído
**Duração:** 3 semanas
**Data Conclusão:** 12/11/2025

### Objetivos
Criar a base sólida do projeto com Clean Architecture e infraestrutura completa.

### Entregas

#### 1.1 Estrutura do Projeto ✅
- [x] Criar solution .NET 9
- [x] Criar 6 projetos (Api, Domain, Data, Externals, CrossCutting, IoC)
- [x] Configurar referências entre projetos
- [x] Estrutura de pastas organizada

#### 1.2 Domain Layer ✅
- [x] Entity `TabelaDinamica` com DDD
- [x] Value Objects (CampoTabela, VinculoTabela)
- [x] Interfaces de repositórios
- [x] Interfaces de serviços
- [x] Validações de domínio
- [x] Factory methods

#### 1.3 Infrastructure Layer ✅
- [x] MetadadosRepository com Dapper
- [x] Configuração de banco Oracle
- [x] DatabaseSettings
- [x] Dependency Injection configurado
- [x] Connection management

#### 1.4 API Layer ✅
- [x] MetadadosController básico
- [x] Swagger/OpenAPI configurado
- [x] DTOs de request/response
- [x] Tratamento de erros
- [x] Logging básico

#### 1.5 Banco de Dados ✅
- [x] Script de criação da TABELA_DINAMICA
- [x] Dados de exemplo (6 tabelas)
- [x] Índices otimizados
- [x] Scripts de verificação

#### 1.6 Docker & DevOps ✅
- [x] docker-compose.yaml completo
- [x] Dockerfile da API
- [x] Scripts PowerShell de automação
- [x] VS Code tasks configuradas
- [x] Launch configurations

#### 1.7 Documentação ✅
- [x] README principal
- [x] Documentação técnica (docs/)
- [x] api-tests.http
- [x] Estrutura docs/projeto/

---

## 🚧 Fase 2: Funcionalidades Core (EM ANDAMENTO)

**Status:** 🚧 20% Concluído
**Prazo Estimado:** 4 semanas
**Data Prevista:** 10/12/2025

### Objetivos
Implementar o coração do sistema - geração dinâmica de queries.

### 2.1 Query Builder Service 🎯 PRÓXIMO
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐⭐⭐
Tempo estimado: 1 semana
```

**Tarefas:**
- [ ] Criar `QueryBuilderService.cs` no Domain/Services
- [ ] Implementar `MontarQuery(nomeTabela, incluirJoins)`
- [ ] Lógica de parsing de vínculos
- [ ] Geração de JOINs automáticos
- [ ] Suporte a profundidade de JOINs
- [ ] Prevenção de loops infinitos
- [ ] Testes unitários do serviço

**Implementação:**
```csharp
public class QueryBuilderService : IQueryBuilderService
{
    Task<Query> MontarQueryAsync(string nomeTabela, bool incluirJoins = false);
    Task<Query> MontarQueryComFiltrosAsync(string nomeTabela, Dictionary<string, object> filtros);
    Task<Query> MontarQueryComOrdenacaoAsync(string nomeTabela, string campoOrdenacao, bool desc = false);
    Task<Query> MontarQueryComPaginacaoAsync(string nomeTabela, int pagina, int itensPorPagina);
}
```

### 2.2 Consulta Dinâmica Repository
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐⭐
Tempo estimado: 3 dias
```

**Tarefas:**
- [ ] Criar `ConsultaDinamicaRepository.cs`
- [ ] Implementar `ExecutarQueryAsync<T>(Query)`
- [ ] Mapeamento dinâmico de resultados
- [ ] Tratamento de tipos Oracle
- [ ] Timeout configurável
- [ ] Testes de integração

**Implementação:**
```csharp
public class ConsultaDinamicaRepository : IConsultaDinamicaRepository
{
    Task<IEnumerable<dynamic>> ExecutarQueryAsync(Query query);
    Task<T> ExecutarQuerySingleAsync<T>(Query query);
    Task<int> ExecutarCountAsync(Query query);
}
```

### 2.3 Consulta Dinâmica Controller
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐
Tempo estimado: 2 dias
```

**Endpoints a criar:**
```http
GET /api/consulta/{tabela}
GET /api/consulta/{tabela}/filtros
GET /api/consulta/{tabela}/count
POST /api/consulta/custom
```

**Tarefas:**
- [ ] Criar `ConsultaDinamicaController.cs`
- [ ] Endpoint GET simples
- [ ] Endpoint com filtros
- [ ] Endpoint com ordenação
- [ ] Endpoint com paginação
- [ ] Endpoint de contagem
- [ ] Validações de segurança (WhiteList)
- [ ] Rate limiting

### 2.4 Filtros Dinâmicos
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐⭐
Tempo estimado: 3 dias
```

**Tarefas:**
- [ ] Parser de filtros do query string
- [ ] Suporte a operadores (=, >, <, LIKE, IN)
- [ ] Filtros AND/OR
- [ ] Validação de campos contra metadados
- [ ] Prevenção de SQL injection
- [ ] Testes de segurança

**Exemplo de uso:**
```http
GET /api/consulta/CLIENTES?nome__like=%João%&ativo=1&cidade__in=SP,RJ
```

### 2.5 Ordenação Dinâmica
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐
Tempo estimado: 1 dia
```

**Tarefas:**
- [ ] Suporte a ORDER BY dinâmico
- [ ] Múltiplos campos de ordenação
- [ ] ASC/DESC configurável
- [ ] Validação de campos

**Exemplo:**
```http
GET /api/consulta/PRODUTOS?orderBy=preco&desc=true
GET /api/consulta/PRODUTOS?orderBy=categoria,nome
```

### 2.6 Paginação
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐
Tempo estimado: 1 dia
```

**Tarefas:**
- [ ] Suporte a LIMIT/OFFSET
- [ ] Metadados de paginação na resposta
- [ ] Links de navegação (HATEOAS)
- [ ] Configuração de limite máximo

**Resposta:**
```json
{
  "dados": [...],
  "paginacao": {
    "paginaAtual": 1,
    "totalPaginas": 10,
    "itensPorPagina": 20,
    "totalItens": 200
  },
  "links": {
    "proxima": "/api/consulta/CLIENTES?pagina=2",
    "anterior": null
  }
}
```

---

## ⏳ Fase 3: Qualidade & Performance

**Status:** ⏳ Planejado
**Prazo Estimado:** 3 semanas
**Data Prevista:** 31/12/2025

### 3.1 Testes Unitários
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐⭐
```

**Tarefas:**
- [ ] Setup do xUnit
- [ ] Testes de TabelaDinamica entity
- [ ] Testes de Value Objects
- [ ] Testes de QueryBuilderService
- [ ] Mocks de repository
- [ ] Cobertura > 80%

**Estrutura:**
```
QueryBuilder.Tests/
├── Domain/
│   ├── Entities/
│   │   └── TabelaDinamicaTests.cs
│   ├── ValueObjects/
│   │   └── ValueObjectsTests.cs
│   └── Services/
│       └── QueryBuilderServiceTests.cs
└── Infrastructure/
    └── Repositories/
        └── MetadadosRepositoryTests.cs
```

### 3.2 Testes de Integração
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐⭐⭐
```

**Tarefas:**
- [ ] Setup de banco de teste
- [ ] WebApplicationFactory para testes
- [ ] Testes end-to-end de endpoints
- [ ] Testes de integração com Oracle
- [ ] CI/CD pipeline básico

### 3.3 Cache de Metadados
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐⭐
```

**Tarefas:**
- [ ] Implementar IMemoryCache
- [ ] Cache decorator para repository
- [ ] Invalidação de cache
- [ ] Configuração de TTL
- [ ] Métricas de hit/miss

**Implementação:**
```csharp
public class CachedMetadadosRepository : IMetadadosRepository
{
    private readonly IMetadadosRepository _inner;
    private readonly IMemoryCache _cache;

    public async Task<TabelaDinamica?> ObterPorNomeTabelaAsync(string nome)
    {
        return await _cache.GetOrCreateAsync(
            $"metadado_{nome}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await _inner.ObterPorNomeTabelaAsync(nome);
            });
    }
}
```

### 3.4 Logging Estruturado
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐
```

**Tarefas:**
- [ ] Instalar Serilog
- [ ] Configurar sinks (Console, File, Seq)
- [ ] Logging de todas as queries geradas
- [ ] Logging de performance
- [ ] Correlation ID em requests
- [ ] Structured logging

### 3.5 Health Checks
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐
```

**Tarefas:**
- [ ] Health check endpoint
- [ ] Verificação de conexão Oracle
- [ ] Verificação de dependências
- [ ] Métricas de uptime

### 3.6 Métricas & Observabilidade
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐⭐
```

**Tarefas:**
- [ ] Prometheus metrics
- [ ] Application Insights
- [ ] Performance counters
- [ ] Dashboard Grafana

---

## ⏳ Fase 4: Melhorias

**Status:** ⏳ Planejado
**Prazo Estimado:** 4 semanas
**Data Prevista:** 31/01/2026

### 4.1 Autenticação & Autorização
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐⭐⭐
```

**Tarefas:**
- [ ] JWT authentication
- [ ] Identity configurado
- [ ] Roles e claims
- [ ] Policy-based authorization
- [ ] Proteção de endpoints
- [ ] Refresh tokens

### 4.2 Validação de Entrada
```
Prioridade: 🔴 ALTA
Complexidade: ⭐⭐
```

**Tarefas:**
- [ ] FluentValidation instalado
- [ ] Validators para todos os DTOs
- [ ] Mensagens de erro customizadas
- [ ] Validação de regras complexas

### 4.3 Rate Limiting
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐
```

**Tarefas:**
- [ ] AspNetCoreRateLimit configurado
- [ ] Limites por IP
- [ ] Limites por usuário
- [ ] Limites por endpoint
- [ ] Headers de rate limit

### 4.4 CORS Configurável
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐
```

**Tarefas:**
- [ ] Política CORS configurável
- [ ] Origins permitidas via config
- [ ] Métodos e headers configuráveis

### 4.5 Versionamento de API
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐
```

**Tarefas:**
- [ ] API Versioning instalado
- [ ] Versão 1.0 estabelecida
- [ ] Suporte a múltiplas versões
- [ ] Deprecation headers

### 4.6 Endpoints Adicionais
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐
```

**Novos endpoints:**
```http
PUT /api/metadados/{id}           # Atualizar metadado
DELETE /api/metadados/{id}        # Deletar (soft delete)
GET /api/metadados/relacoes       # Grafo de relacionamentos
GET /api/metadados/validar        # Validar metadados
POST /api/metadados/importar      # Importar de JSON
GET /api/metadados/exportar       # Exportar para JSON
```

---

## ⏳ Fase 5: IA & Automação

**Status:** ⏳ Planejado
**Prazo Estimado:** 6 semanas
**Data Prevista:** 15/03/2026

### 5.1 Integração com OpenAI
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐⭐⭐⭐
```

**Tarefas:**
- [ ] OpenAI SDK integrado
- [ ] Geração de contexto estruturado
- [ ] Prompt engineering
- [ ] Parsing de resposta da IA
- [ ] Fallback em caso de erro

### 5.2 Natural Language Queries
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐⭐⭐⭐
```

**Tarefas:**
- [ ] Endpoint para queries em linguagem natural
- [ ] Conversão de texto para Query
- [ ] Validação de segurança
- [ ] Histórico de queries
- [ ] Feedback do usuário

**Exemplo:**
```http
POST /api/consulta/natural
{
  "query": "Liste os clientes de São Paulo com pedidos ativos"
}

Resposta:
{
  "sqlGerado": "SELECT c.* FROM CLIENTES c JOIN...",
  "resultados": [...]
}
```

### 5.3 Sugestões Automáticas
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐⭐⭐
```

**Tarefas:**
- [ ] Análise de performance de queries
- [ ] Sugestão de índices
- [ ] Sugestão de desnormalização
- [ ] Alertas de queries lentas

### 5.4 Documentação Auto-gerada
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐⭐
```

**Tarefas:**
- [ ] Gerar documentação de schema
- [ ] Gerar diagramas ER
- [ ] Gerar exemplos de queries
- [ ] Atualização automática

---

## ⏳ Fase 6: Multi-Database

**Status:** ⏳ Planejado
**Prazo Estimado:** 8 semanas
**Data Prevista:** 15/05/2026

### 6.1 Abstração de Database
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐⭐⭐⭐
```

**Tarefas:**
- [ ] Interface IDatabaseProvider
- [ ] OracleProvider (já existe)
- [ ] Adapter pattern
- [ ] Factory de providers
- [ ] Configuração multi-database

### 6.2 Suporte PostgreSQL
```
Prioridade: 🟡 MÉDIA
Complexidade: ⭐⭐⭐⭐
```

**Tarefas:**
- [ ] PostgreSqlProvider
- [ ] Testes de integração
- [ ] Migração de scripts SQL
- [ ] Documentação específica

### 6.3 Suporte MySQL
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐⭐⭐
```

### 6.4 Suporte SQL Server
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐⭐⭐
```

### 6.5 Ferramenta de Migração
```
Prioridade: 🟢 BAIXA
Complexidade: ⭐⭐⭐⭐⭐
```

**Tarefas:**
- [ ] CLI para migração entre bancos
- [ ] Export/Import de metadados
- [ ] Conversão de tipos
- [ ] Validação de compatibilidade

---

## 📊 Resumo do Roadmap

### Linha do Tempo
```
Nov 2025  [████████████] Fase 1: Fundação ✅
Dez 2025  [███░░░░░░░░░] Fase 2: Core 🚧
Jan 2026  [░░░░░░░░░░░░] Fase 3: Qualidade
Fev 2026  [░░░░░░░░░░░░] Fase 4: Melhorias
Mar 2026  [░░░░░░░░░░░░] Fase 5: IA
Mai 2026  [░░░░░░░░░░░░] Fase 6: Multi-DB
```

### Esforço Total Estimado
```
Fase 1: 3 semanas  ✅
Fase 2: 4 semanas  🚧
Fase 3: 3 semanas  ⏳
Fase 4: 4 semanas  ⏳
Fase 5: 6 semanas  ⏳
Fase 6: 8 semanas  ⏳
────────────────────
Total:  28 semanas (~7 meses)
```

---

<div align="center">

**🗺️ Roadmap claro = Execução focada! 🎯**

[← Voltar ao Índice](00_INDICE.md) | [Próximo: Próximos Passos →](06_PROXIMOS_PASSOS.md)

</div>
