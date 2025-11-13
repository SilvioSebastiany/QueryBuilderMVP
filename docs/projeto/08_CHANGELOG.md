# 📝 Changelog - QueryBuilder MVP

Registro de todas as mudanças notáveis neste projeto.

---

## [0.3.0] - 2025-11-13

### ✨ Adicionado
- **QueryBuilderService completo** - Serviço de geração de queries dinâmicas
  - Método `MontarQuery()` - Gera SELECT com/sem JOINs
  - Método `MontarQueryComFiltros()` - Adiciona cláusulas WHERE dinâmicas
  - Método `MontarQueryComOrdenacao()` - Adiciona ORDER BY
  - Método `MontarQueryComPaginacao()` - Adiciona LIMIT/OFFSET
  - Método `CompilarQuery()` - Compila Query para SQL Oracle
  - Método `ListarTabelas()` - Lista tabelas disponíveis nos metadados
  - Método `TabelaExiste()` - Valida existência de tabela
  - Método `ObterGrafoRelacionamentos()` - Exibe hierarquia de relacionamentos
  - JOINs recursivos com controle de profundidade configurável
  - Prevenção de loops infinitos com HashSet
  - Logging estruturado em todos os métodos

- **QueryBuilderTestController** - Controller para testes e debug
  - `GET /api/QueryBuilderTest/simples/{tabela}` - Gera query sem JOINs
  - `GET /api/QueryBuilderTest/com-joins/{tabela}` - Gera query com JOINs recursivos
  - `POST /api/QueryBuilderTest/com-filtros/{tabela}` - Gera query com filtros WHERE
  - `GET /api/QueryBuilderTest/tabelas-disponiveis` - Lista metadados carregados
  - Parâmetro `profundidade` configurável para controle de JOINs
  - Retorna SQL compilado para debug e validação
  - Tratamento de erros com responses adequados (404, 400)

- **Script create-tables.sql** - Criação completa do schema do e-commerce
  - 6 tabelas relacionadas: CATEGORIAS, CLIENTES, ENDERECOS, PRODUTOS, PEDIDOS, ITENS_PEDIDO
  - Foreign Keys e constraints de integridade
  - Índices para otimização de queries
  - 35 registros de dados de exemplo
  - Comentários em todas as colunas
  - Auto-increment com IDENTITY
  - Consulta de verificação final

- **querybuilder-tests.http** - Arquivo de testes HTTP
  - 20+ casos de teste cobrindo todos os endpoints
  - Testes de queries simples (sem JOINs)
  - Testes de queries com JOINs (profundidades 1, 2, 3)
  - Testes de queries com filtros
  - Testes de validação de erros
  - Seções organizadas por funcionalidade

### 🔧 Modificado
- **DependencyInjection.cs**
  - Adicionado registro de `IQueryBuilderService` → `QueryBuilderService` (Scoped)
  - Adicionado registro de `OracleCompiler` (Singleton)
  - Importado namespace `SqlKata.Compilers`

- **QueryBuilder.Domain.csproj**
  - Adicionado pacote `Microsoft.Extensions.Logging.Abstractions` v9.0.0

- **docker-compose.yaml**
  - Removido healthcheck do serviço oracle-db
  - Removido script de inicialização automática (agora manual)
  - Simplificada dependência entre containers

- **Documentação**
  - Atualizado `docs/projeto/04_STATUS_ATUAL.md` com progresso de 35% → 55%
  - Atualizada seção "Consultas Dinâmicas" para refletir implementações completas
  - Adicionadas estatísticas de código atualizadas
  - Adicionados testes manuais realizados

### 📊 Estatísticas
- **Linhas de código:** 4.100 → 6.660 (+2.560 linhas)
- **Arquivos criados:** 35 → 42 (+7 arquivos)
- **Progresso geral:** 35% → 55% (+20%)
- **Domain Layer:** 400 → 750 linhas
- **API Layer:** 200 → 380 linhas
- **Scripts SQL:** 200 → 650 linhas

---

## [0.2.0] - 2025-11-12

### ✨ Adicionado
- **Estrutura completa do projeto**
  - 6 projetos .NET 9.0 organizados em Clean Architecture
  - Solution `QueryBuilder.Solution.sln`

- **Domain Layer**
  - Entity `TabelaDinamica` com DDD (agregado raiz)
  - Value Objects (`CampoTabela`, `VinculoTabela`, `MetadadoDescricao`)
  - Interfaces de repositórios e serviços
  - Validações de domínio

- **Infrastructure Layer**
  - `MetadadosRepository` completo com Dapper
  - Conexão com Oracle Database
  - `DatabaseSettings` para configurações
  - Dependency Injection configurado

- **API Layer**
  - `MetadadosController` com 5 endpoints
  - Swagger configurado
  - Logging estruturado
  - Program.cs com pipeline completo

- **Banco de Dados**
  - Script `init-database.sql` com tabela TABELA_DINAMICA
  - 6 registros de metadados de exemplo
  - Índices otimizados
  - Scripts auxiliares de verificação

- **Docker**
  - `docker-compose.yaml` com Oracle XE e API
  - Dockerfile multi-stage para API
  - Volumes para persistência
  - Healthchecks configurados

- **DevOps**
  - `debug-manager.ps1` - Script PowerShell de gerenciamento
  - Tasks do VS Code para build, test, docker
  - Launch configurations para debug

- **Documentação**
  - README.md principal completo
  - Pasta `docs/projeto/` estruturada
  - 7 documentos de arquitetura e planejamento
  - Guias de Docker e comandos

### 🧪 Testado
- Build da solution sem erros
- API rodando em http://localhost:5249
- Swagger acessível em /swagger
- Conexão com Oracle funcionando
- Metadados sendo consultados corretamente
- Docker containers saudáveis

---

## [0.1.0] - 2025-11-10

### ✨ Adicionado
- Repositório inicial criado
- Estrutura básica de pastas
- .gitignore configurado
- Primeiros commits

---

## 📋 Legenda

- ✨ **Adicionado** - Novas funcionalidades
- 🔧 **Modificado** - Alterações em funcionalidades existentes
- 🐛 **Corrigido** - Correções de bugs
- 🗑️ **Removido** - Funcionalidades removidas
- 📝 **Documentação** - Apenas alterações de documentação
- 🔒 **Segurança** - Vulnerabilidades corrigidas
- ⚡ **Performance** - Melhorias de desempenho
- 🧪 **Testes** - Adição ou modificação de testes

---

## 🔗 Links Úteis

- [Roadmap Completo](05_ROADMAP.md)
- [Status Atual](04_STATUS_ATUAL.md)
- [Próximos Passos](06_PROXIMOS_PASSOS.md)
- [Voltar ao Índice](00_INDICE.md)

---

<div align="center">

**Última atualização:** 13 de Novembro de 2025

</div>
