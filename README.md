# QueryBuilder MVP - Clean Architecture

Sistema de consultas dinâmicas ao banco de dados Oracle utilizando SqlKata e Clean Architecture/DDD.

## 🏗️ Arquitetura

```
QueryBuilderMVP/
├── src/
│   ├── QueryBuilder.Api/              # API REST (Controllers, Program.cs)
│   ├── QueryBuilder.Domain/           # Entidades, ValueObjects, Interfaces
│   ├── QueryBuilder.Infra.Data/       # Repositórios, Dapper, Oracle
│   ├── QueryBuilder.Infra.CrossCutting/      # Configurações compartilhadas
│   ├── QueryBuilder.Infra.CrossCutting.IoC/  # Injeção de Dependência
│   └── QueryBuilder.Infra.Externals/  # Serviços externos
├── scripts/                           # Scripts SQL de inicialização
├── docs/                              # Documentação completa
├── docker-compose.yaml                # Orquestração Docker
├── debug-manager.ps1                  # Gerenciamento de debug
└── QueryBuilder.Solution.sln          # Solution principal

```

## 🚀 Quick Start

### Pré-requisitos
- .NET 9.0 SDK
- Docker Desktop
- VS Code com C# Dev Kit

### 1. Iniciar Ambiente Docker
```powershell
docker compose up -d
```

### 2. Inicializar Banco de Dados
```powershell
# Método 1: Via VS Code Tasks
# Ctrl+Shift+P → Tasks: Run Task → setup-database

# Método 2: Via PowerShell
docker exec -i querybuilder-oracle-xe sqlplus system/oracle@XE '@/tmp/init-database.sql'
```

### 3. Rodar a API

**Opção A: Debug no VS Code (Recomendado)**
```
Pressione F5
```

**Opção B: Terminal**
```powershell
dotnet run --project src/QueryBuilder.Api/QueryBuilder.Api.csproj
```

### 4. Testar Endpoints

**Swagger UI**: http://localhost:5249/swagger

**Ou use o arquivo `api-tests.http`** (REST Client extension)

## 🔧 Gerenciamento de Debug

```powershell
# Verificar status completo
.\debug-manager.ps1 status

# Liberar porta 5249 para debug local
.\debug-manager.ps1 free

# Parar containers Docker
.\debug-manager.ps1 docker-down

# Iniciar containers Docker
.\debug-manager.ps1 docker-up
```

## 📊 Tecnologias

- **.NET 9.0** - Framework principal
- **ASP.NET Core Web API** - REST API
- **SqlKata 4.0.1** - Query Builder
- **Dapper 2.1.66** - Micro ORM
- **Oracle Database 21c XE** - Banco de dados
- **Docker** - Containerização
- **Clean Architecture** - Padrão arquitetural
- **DDD** - Domain-Driven Design

## 📁 Estrutura de Camadas

### 1. Domain Layer (`QueryBuilder.Domain`)
- **Entities**: `TabelaDinamica` (agregado raiz)
- **ValueObjects**: `CampoTabela`, `VinculoTabela`, `MetadadoDescricao`
- **Interfaces**: Contratos de repositórios e serviços

### 2. Infrastructure Layer
- **Infra.Data**: Implementação de repositórios com Dapper + Oracle
- **Infra.CrossCutting**: Configurações e utilitários
- **Infra.CrossCutting.IoC**: Configuração de DI
- **Infra.Externals**: Integrações externas

### 3. API Layer (`QueryBuilder.Api`)
- Controllers REST
- Configuração de Swagger
- Middleware pipeline

## 🗄️ Banco de Dados

**Connection String**: `User Id=SYSTEM;Password=oracle;Data Source=localhost:1522/XE`

**Porta Oracle**: `1522` (mapeada para 1521 interno do container)

**Tabela Principal**: `TABELA_DINAMICA`
- ID (NUMBER PRIMARY KEY)
- TABELA (VARCHAR2)
- CAMPOS_DISPONIVEIS (CLOB - JSON)
- CHAVE_PK (VARCHAR2)
- VINCULO_ENTRE_TABELA (CLOB - JSON)
- DESCRICAO_TABELA (CLOB)
- DESCRICAO_CAMPOS (CLOB - JSON)
- VISIVEL_PARA_IA (NUMBER(1))
- DATA_CRIACAO (TIMESTAMP)
- DATA_ATUALIZACAO (TIMESTAMP)
- ATIVO (NUMBER(1))

## 🎯 Endpoints Disponíveis

### Metadados
- `GET /api/metadados/teste` - Endpoint de teste
- `GET /api/metadados` - Listar todos os metadados
- `GET /api/metadados/{id}` - Buscar por ID
- `GET /api/metadados/tabela/{nome}` - Buscar por nome da tabela
- `POST /api/metadados` - Criar novo metadado

## 📖 Documentação

Toda documentação está na pasta `docs/`:
- **DEBUG_README.md** - Guia completo de debug
- **DOCKER_README.md** - Operações Docker
- **ORACLE_CONFIGURADO.md** - Solução de problemas Oracle
- **MVP_TESTE.md** - Guia de testes do MVP
- E mais...

## 🐛 Troubleshooting

### Porta 5249 em uso
```powershell
.\debug-manager.ps1 free
```

### Oracle não conecta
1. Verificar se porta 1522 está correta no `appsettings.json`
2. Confirmar que container está healthy: `docker ps`
3. Reinicializar banco: `.\debug-manager.ps1 docker-down` + `.\debug-manager.ps1 docker-up`

### Tabela TABELA_DINAMICA não existe
```powershell
# Via VS Code
Ctrl+Shift+P → Tasks: Run Task → setup-database

# Via PowerShell
docker cp scripts/init-database.sql querybuilder-oracle-xe:/tmp/
docker exec -i querybuilder-oracle-xe sqlplus system/oracle@XE '@/tmp/init-database.sql'
```

## 📝 Próximos Passos

- [ ] Implementar `QueryBuilderService` (montagem de queries dinâmicas)
- [ ] Implementar `ConsultaDinamicaRepository` (execução de queries)
- [ ] Criar `ConsultaDinamicaController` (endpoint de consulta dinâmica)
- [ ] Adicionar testes unitários
- [ ] Implementar cache de metadados
- [ ] Adicionar logging estruturado

## 📄 Licença

MIT License - veja arquivo LICENSE na raiz original do SqlKata

## 👥 Autor

Projeto criado para aprendizado e uso de SqlKata com Clean Architecture e Oracle Database.
