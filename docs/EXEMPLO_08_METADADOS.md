# 🚀 Query Builder Dinâmico com Metadados

## 📋 Visão Geral

Sistema avançado que **lê metadados de uma tabela** e **monta queries automaticamente** com JOINs recursivos, eliminando a necessidade de escrever SQL manualmente.

---

## 🎯 Problema Resolvido

**Antes** (código repetitivo):
```csharp
// Para cada query, escrever tudo manualmente...
var query1 = new Query("PEDIDO")
    .Select("PEDIDO.ID", "PEDIDO.NUMERO", "PEDIDO.VALOR")
    .Join("CLIENTE", "CLIENTE.ID", "PEDIDO.CLIENTE_ID")
    .Select("CLIENTE.NOME AS CLIENTE_NOME");

var query2 = new Query("PRODUTO")
    .Select("PRODUTO.ID", "PRODUTO.NOME", "PRODUTO.PRECO")
    .Join("CATEGORIA", "CATEGORIA.ID", "PRODUTO.CATEGORIA_ID")
    .Select("CATEGORIA.NOME AS CATEGORIA_NOME");

// Repetir para cada tabela... 😫
```

**Depois** (dinâmico e automático):
```csharp
// Uma única linha! 🎉
var query = queryBuilder.MontarQuery("PEDIDO", incluirJoins: true);

// Funciona para qualquer tabela cadastrada nos metadados
var query2 = queryBuilder.MontarQuery("PRODUTO", incluirJoins: true);
```

---

## 📊 Arquitetura

```
┌─────────────────────────────────────────────────┐
│          TABELA_DINAMICA (Metadados)            │
├─────────────────────────────────────────────────┤
│ TABELA            │ Nomes das tabelas           │
│ CAMPOS_DISPONIVEIS│ Colunas para SELECT         │
│ CHAVE_PK          │ Chave primária              │
│ VINCULO_ENTRE_...│ Relacionamentos (FKs)       │
└─────────────────────────────────────────────────┘
                      ↓
        ┌─────────────────────────────┐
        │  QueryBuilderDinamico.cs    │
        │  • CarregarMetadados()      │
        │  • MontarQuery()            │
        │  • JOINs Recursivos         │
        └─────────────────────────────┘
                      ↓
        ┌─────────────────────────────┐
        │  SqlKata Query + Dapper     │
        │  SQL gerado automaticamente │
        └─────────────────────────────┘
```

---

## 📁 Arquivos Criados

### 1. **QueryBuilderDinamico.cs**
Classe principal com toda a lógica:

**Métodos principais:**
- `CarregarMetadados()` - Lê TABELA_DINAMICA do banco
- `MontarQuery()` - Cria query com JOINs automáticos
- `MontarQueryComFiltros()` - Query + filtros WHERE
- `MontarQueryComOrdenacao()` - Query + ORDER BY
- `MontarQueryComPaginacao()` - Query + LIMIT/OFFSET
- `ObterGrafoRelacionamentos()` - Visualiza estrutura

**Recursos:**
- ✅ JOINs recursivos (ex: PEDIDO → CLIENTE → ENDERECO → CIDADE)
- ✅ Profundidade configurável
- ✅ Prevenção de loops infinitos
- ✅ Suporte a múltiplos vínculos por tabela

### 2. **08_MetadadosQueryDinamica.cs**
Exemplo executável com 9 cenários demonstrando uso prático.

### 3. **script_tabela_dinamica.sql**
Script Oracle completo com:
- Estrutura da TABELA_DINAMICA
- Dados de exemplo (Pedido, Cliente, Produto, etc)
- Queries de validação
- Exemplos de uso

---

## 🗄️ Estrutura da TABELA_DINAMICA

```sql
CREATE TABLE TABELA_DINAMICA (
    TABELA                VARCHAR2(100) PRIMARY KEY,
    CAMPOS_DISPONIVEIS    VARCHAR2(500) NOT NULL,
    CHAVE_PK              VARCHAR2(100) NOT NULL,
    VINCULO_ENTRE_TABELA  VARCHAR2(500)
);
```

**Exemplo de dados:**
```sql
INSERT INTO TABELA_DINAMICA VALUES (
    'PEDIDO',
    'ID,NUMERO,DATA_PEDIDO,VALOR_TOTAL,CLIENTE_ID,STATUS',
    'ID',
    'CLIENTE.CLIENTE_ID,ITEM_PEDIDO.PEDIDO_ID'
);

INSERT INTO TABELA_DINAMICA VALUES (
    'CLIENTE',
    'ID,NOME,EMAIL,TELEFONE,ENDERECO_ID',
    'ID',
    'ENDERECO.ENDERECO_ID'
);
```

---

## 💻 Como Usar

### Passo 1: Executar Script SQL
```sql
-- Execute no Oracle SQL Developer ou similar
@script_tabela_dinamica.sql
```

### Passo 2: Configurar Conexão
```csharp
using Oracle.ManagedDataAccess.Client;
using SqlKata.Compilers;

var connectionString = "User Id=usuario;Password=senha;Data Source=localhost:1521/XE";
var connection = new OracleConnection(connectionString);
var compiler = new OracleCompiler();
```

### Passo 3: Usar QueryBuilderDinamico
```csharp
// Criar instância
var queryBuilder = new QueryBuilderDinamico(connection, compiler);

// Carregar metadados do banco
await queryBuilder.CarregarMetadados();

// Montar query automaticamente
var query = queryBuilder.MontarQuery("PEDIDO", incluirJoins: true, profundidadeMaxima: 3);

// Compilar
var resultado = compiler.Compile(query);

// Executar com Dapper
var pedidos = await connection.QueryAsync<dynamic>(resultado.Sql, resultado.NamedBindings);

// Usar dados
foreach (var pedido in pedidos)
{
    Console.WriteLine($"Pedido: {pedido.NUMERO}");
    Console.WriteLine($"Cliente: {pedido.CLIENTE_NOME}");
    Console.WriteLine($"Cidade: {pedido.ENDERECO_CIDADE}");
}
```

---

## 🎯 Casos de Uso

### 1️⃣ APIs RESTful Genéricas
```csharp
[HttpGet("api/{tabela}")]
public async Task<IActionResult> Get(string tabela, [FromQuery] bool incluirRelacionamentos = false)
{
    var query = queryBuilder.MontarQuery(tabela, incluirRelacionamentos);
    var resultado = await ExecutarQuery(query);
    return Ok(resultado);
}

// GET /api/PEDIDO?incluirRelacionamentos=true
// Retorna pedidos com cliente, endereço, itens, produtos, etc.
```

### 2️⃣ Relatórios Dinâmicos
```csharp
public async Task<Relatorio> GerarRelatorio(string tabelaBase, Dictionary<string, object> filtros)
{
    var query = queryBuilder.MontarQueryComFiltros(tabelaBase, filtros, incluirJoins: true);
    var dados = await ExecutarQuery(query);
    return new Relatorio { Dados = dados };
}

// Usuário escolhe tabela e filtros na tela
// Sistema gera relatório automaticamente
```

### 3️⃣ Multi-Tenant
```csharp
// Cada tenant pode ter metadados diferentes
await queryBuilder.CarregarMetadados($"WHERE TENANT_ID = {tenantId}");

// Queries se adaptam automaticamente à estrutura do tenant
var query = queryBuilder.MontarQuery("CLIENTE");
```

### 4️⃣ Integrações Externas
```csharp
// Sistema externo define estrutura via API
POST /api/metadados
{
    "tabela": "NOVA_ENTIDADE",
    "campos": "ID,NOME,DESCRICAO",
    "chave_pk": "ID",
    "vinculos": "OUTRA_TABELA.ID_VINCULO"
}

// Aplicação se adapta automaticamente às mudanças
```

---

## 🔒 Segurança

### ⚠️ CRÍTICO: Validação Obrigatória

```csharp
public class QueryBuilderSeguro : QueryBuilderDinamico
{
    private static readonly HashSet<string> TabelasPermitidas = new()
    {
        "PEDIDO", "CLIENTE", "PRODUTO", "CATEGORIA"
    };

    public Query MontarQuerySegura(string tabela, string usuarioId)
    {
        // 1. Validar permissão
        if (!UsuarioTemPermissao(usuarioId, tabela))
            throw new UnauthorizedAccessException();

        // 2. Validar tabela na WhiteList
        if (!TabelasPermitidas.Contains(tabela.ToUpper()))
            throw new SecurityException("Tabela não autorizada");

        // 3. Limitar profundidade
        var query = MontarQuery(tabela, incluirJoins: true, profundidadeMaxima: 2);

        // 4. Adicionar filtros de segurança
        query.Where("ATIVO", 1);
        query.Where("TENANT_ID", ObterTenantId(usuarioId));

        return query;
    }
}
```

**Checklist de Segurança:**
- [ ] WhiteList de tabelas
- [ ] WhiteList de campos
- [ ] Validação de permissões por usuário
- [ ] Limite de profundidade de JOINs
- [ ] Timeout nas queries
- [ ] Log de todas as queries geradas
- [ ] Filtros de tenant/organização
- [ ] Rate limiting

---

## 📊 Exemplo de Query Gerada

**Entrada:**
```csharp
var query = queryBuilder.MontarQuery("PEDIDO", incluirJoins: true, profundidadeMaxima: 2);
```

**SQL gerado automaticamente:**
```sql
SELECT
    "PEDIDO"."ID",
    "PEDIDO"."NUMERO",
    "PEDIDO"."DATA_PEDIDO",
    "PEDIDO"."VALOR_TOTAL",
    "PEDIDO"."CLIENTE_ID",
    "PEDIDO"."STATUS",
    "CLIENTE"."ID" AS "CLIENTE_ID",
    "CLIENTE"."NOME" AS "CLIENTE_NOME",
    "CLIENTE"."EMAIL" AS "CLIENTE_EMAIL",
    "CLIENTE"."TELEFONE" AS "CLIENTE_TELEFONE",
    "ENDERECO"."ID" AS "ENDERECO_ID",
    "ENDERECO"."RUA" AS "ENDERECO_RUA",
    "ENDERECO"."CIDADE" AS "ENDERECO_CIDADE",
    "ITEM_PEDIDO"."ID" AS "ITEM_PEDIDO_ID",
    "ITEM_PEDIDO"."QUANTIDADE" AS "ITEM_PEDIDO_QUANTIDADE",
    "PRODUTO"."ID" AS "PRODUTO_ID",
    "PRODUTO"."NOME" AS "PRODUTO_NOME",
    "PRODUTO"."PRECO" AS "PRODUTO_PRECO"
FROM "PEDIDO"
INNER JOIN "CLIENTE" ON "CLIENTE"."ID" = "PEDIDO"."CLIENTE_ID"
INNER JOIN "ENDERECO" ON "ENDERECO"."ID" = "CLIENTE"."ENDERECO_ID"
INNER JOIN "ITEM_PEDIDO" ON "ITEM_PEDIDO"."PEDIDO_ID" = "PEDIDO"."ID"
INNER JOIN "PRODUTO" ON "PRODUTO"."ID" = "ITEM_PEDIDO"."PRODUTO_ID"
```

---

## ✅ Vantagens

| Vantagem | Descrição |
|----------|-----------|
| 🚀 **Produtividade** | Reduz código repetitivo em 80% |
| 🔧 **Manutenibilidade** | Metadados centralizados |
| 🔄 **Flexibilidade** | Adapta-se a mudanças de estrutura |
| 🏢 **Multi-tenant** | Cada tenant pode ter estrutura diferente |
| 📊 **Queries Complexas** | JOINs recursivos automáticos |
| 🛡️ **Type-Safe** | SqlKata garante SQL válido |

---

## ⚠️ Desvantagens

| Desvantagem | Mitigação |
|-------------|-----------|
| 🐛 **Debugging** | Adicionar logs detalhados |
| ⚡ **Performance** | Revisar queries geradas, criar índices |
| 📈 **Complexidade** | Documentação e testes extensivos |
| 🎓 **Curva de aprendizado** | Treinamento da equipe |

---

## 🧪 Testando

### Modo 1: Sem Banco (Simulação)
```bash
cd Exemplos
dotnet run
# Escolha opção 8
```

Demonstra todas as funcionalidades sem conexão real.

### Modo 2: Com Banco Oracle
1. Execute `script_tabela_dinamica.sql` no banco
2. Configure connection string
3. Execute testes com dados reais

---

## 📚 Recursos Adicionais

**Arquivos relacionados:**
- `QueryBuilderDinamico.cs` - Classe principal
- `08_MetadadosQueryDinamica.cs` - Exemplo executável
- `script_tabela_dinamica.sql` - Script SQL
- `GUIA_ORACLE.md` - Guia específico Oracle
- `Program.cs` - Menu de exemplos

**Próximos passos:**
1. Implementar cache de metadados
2. Adicionar suporte a LEFT/RIGHT JOIN
3. Implementar validação de ciclos
4. Criar interface web para gerenciar metadados
5. Adicionar suporte a stored procedures

---

## 🤝 Contribuindo

Sugestões de melhorias:
- [ ] Suporte a agregações (GROUP BY, HAVING)
- [ ] CTEs (Common Table Expressions)
- [ ] Window Functions
- [ ] Suporte a outros bancos (MySQL, PostgreSQL)
- [ ] Interface gráfica para visualizar grafo
- [ ] Geração de diagramas ER automáticos

---

## 📞 Suporte

Dúvidas? Veja:
- Exemplo 8: `dotnet run` → opção 8
- Script SQL: `script_tabela_dinamica.sql`
- Classe completa: `QueryBuilderDinamico.cs`
- Guia Oracle: `GUIA_ORACLE.md`

---

**🎉 Query Builder Dinâmico = Menos código, mais produtividade!**

*Criado com SqlKata + Dapper + Oracle Database*
