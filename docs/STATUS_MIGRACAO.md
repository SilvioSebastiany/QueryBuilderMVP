# 📋 Status da Migração para Clean Architecture

## ✅ Concluído até agora:

### 1. Estrutura de Solution
- ✅ Criada solution `QueryBuilder.Solution.sln`
- ✅ 6 projetos criados e adicionados à solution:
  - `QueryBuilder.Api` (Web API)
  - `QueryBuilder.Domain` (Regras de negócio)
  - `QueryBuilder.Infra.Data` (Acesso a dados)
  - `QueryBuilder.Infra.Externals` (APIs externas)
  - `QueryBuilder.Infra.CrossCutting` (Recursos compartilhados)
  - `QueryBuilder.Infra.CrossCutting.IoC` (Injeção de dependência)

### 2. Referências entre Projetos
```
Api → Domain, IoC
Infra.Data → Domain, CrossCutting
Infra.Externals → Domain, CrossCutting
IoC → Domain, Infra.Data, Infra.Externals, CrossCutting
```

### 3. Camada Domain (Parcial)
- ✅ **Entities/TabelaDinamica.cs**
  - Entity com encapsulamento DDD
  - Factory methods
  - Validações de domínio
  - Métodos auxiliares

- ✅ **Interfaces/IRepositories.cs**
  - `IMetadadosRepository`
  - `IQueryBuilderService`
  - `IIADataCatalogService`
  - `IValidacaoMetadadosService`
  - `IConsultaDinamicaRepository`

- ✅ **ValueObjects/MetadadosValueObjects.cs**
  - `CampoTabela`
  - `VinculoTabela`
  - `MetadadoDescricao`
  - Enum `TipoJoin`

### 4. Estrutura de Pastas
```
src/
├── QueryBuilder.Domain/
│   ├── Entities/         ✅
│   ├── ValueObjects/     ✅
│   ├── Services/         📁 (criada, vazia)
│   ├── Interfaces/       ✅
│   └── Commands/         📁 (criada, vazia)
│       └── Handlers/     📁 (criada, vazia)
├── QueryBuilder.Api/     📁 (criada, padrão)
├── QueryBuilder.Infra.Data/     📁 (criada, vazia)
├── QueryBuilder.Infra.Externals/     📁 (criada, vazia)
├── QueryBuilder.Infra.CrossCutting/  📁 (criada, vazia)
└── QueryBuilder.Infra.CrossCutting.IoC/  📁 (criada, vazia)
```

---

## 🚧 Próximos Passos:

### Fase 1: Completar Domain Layer
1. **Services** (3 arquivos):
   - [ ] `QueryBuilderService.cs` - Lógica de montagem de queries
   - [ ] `IADataCatalogService.cs` - Geração de contexto para IA
   - [ ] `ValidacaoMetadadosService.cs` - Validações complexas

2. **Commands & Handlers** (CQRS):
   - [ ] `CriarMetadadoCommand.cs`
   - [ ] `AtualizarMetadadoCommand.cs`
   - [ ] `ExecutarConsultaDinamicaCommand.cs`
   - [ ] Handlers correspondentes

### Fase 2: Infra.Data Layer
1. **Repositories**:
   - [ ] `MetadadosRepository.cs` (Dapper)
   - [ ] `ConsultaDinamicaRepository.cs` (SqlKata + Dapper)

2. **Context**:
   - [ ] `OracleDbContext.cs`
   - [ ] Connection string management

3. **Migrations**:
   - [ ] `001_CriarTabelaDinamica.sql`
   - [ ] `002_AdicionarCamposIA.sql`

### Fase 3: Infra.CrossCutting Layer
1. **Settings**:
   - [ ] `DatabaseSettings.cs`
   - [ ] `IASettings.cs`

2. **Extensions**:
   - [ ] `StringExtensions.cs`
   - [ ] `QueryExtensions.cs`

3. **Providers**:
   - [ ] `OracleConnectionProvider.cs`

4. **Enums & Constants**:
   - [ ] `MetadadosConstants.cs`
   - [ ] `StatusMetadado.cs`

### Fase 4: API Layer
1. **Controllers**:
   - [ ] `MetadadosController.cs` (CRUD)
   - [ ] `ConsultaDinamicaController.cs` (Execução)
   - [ ] `IAAssistantController.cs` (Integração IA)

2. **Mappers**:
   - [ ] `MetadadosMapper.cs` (AutoMapper)

3. **Responses/Requests**:
   - [ ] DTOs de entrada e saída

4. **Configuração**:
   - [ ] `appsettings.json`
   - [ ] Swagger/OpenAPI
   - [ ] CORS, Auth, etc

### Fase 5: IoC Container
- [ ] `DependencyInjection.cs` - Registrar todas as dependências

### Fase 6: Infra.Externals (Integração IA)
- [ ] `OpenAIService.cs`
- [ ] Mappers e DTOs

### Fase 7: Testes
- [ ] Unit tests
- [ ] Integration tests

---

## 📊 Progresso Geral
```
[████████░░░░░░░░░░░░░░] 30% Concluído

✅ Estrutura base: 100%
✅ Domain Entities: 100%
✅ Domain Interfaces: 100%
✅ Domain ValueObjects: 100%
⏳ Domain Services: 0%
⏳ Domain Commands: 0%
⏳ Infra.Data: 0%
⏳ API: 0%
⏳ IoC: 0%
⏳ CrossCutting: 0%
```

---

## 💡 Decisões Arquiteturais Tomadas:

1. **DDD**: Entities com encapsulamento, factory methods, validações
2. **CQRS**: Separação de Commands (escrita) e Queries (leitura)
3. **Repository Pattern**: Abstração de acesso a dados
4. **Dependency Injection**: Inversão de controle via IoC container
5. **Value Objects**: Objetos imutáveis para conceitos do domínio
6. **.NET 8.0**: Versão mais recente
7. **Dapper**: Micro-ORM leve para Oracle
8. **SqlKata**: Query builder fluente

---

## 🎯 Próximo Comando:

Para continuar, vou implementar:
1. ✅ Domain Services (3 arquivos)
2. ✅ Commands & Handlers
3. ✅ Infra.Data Repositories
4. ✅ IoC Configuration
5. ✅ API Controllers básicos

**Quer que eu continue agora?** Posso criar todos os arquivos restantes em sequência! 🚀
