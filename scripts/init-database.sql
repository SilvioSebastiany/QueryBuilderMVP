

-- ============================================
-- 2. CRIAR TABELA DE METADADOS
-- ============================================

CREATE TABLE TABELA_DINAMICA (
    ID                    NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    TABELA                VARCHAR2(100) UNIQUE NOT NULL,
    CAMPOS_DISPONIVEIS    VARCHAR2(1000) NOT NULL,
    CHAVE_PK              VARCHAR2(100) NOT NULL,
    VINCULO_ENTRE_TABELA  VARCHAR2(1000),
    DESCRICAO_TABELA      VARCHAR2(500),
    DESCRICAO_CAMPOS      VARCHAR2(2000),
    VISIVEL_PARA_IA       NUMBER(1) DEFAULT 1 NOT NULL,
    DATA_CRIACAO          TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    DATA_ATUALIZACAO      TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    ATIVO                 NUMBER(1) DEFAULT 1 NOT NULL
);

-- Comentários
COMMENT ON TABLE TABELA_DINAMICA IS 'Metadados para geração dinâmica de queries - MVP QueryBuilder';
COMMENT ON COLUMN TABELA_DINAMICA.ID IS 'Identificador único do metadado';
COMMENT ON COLUMN TABELA_DINAMICA.TABELA IS 'Nome da tabela';
COMMENT ON COLUMN TABELA_DINAMICA.CAMPOS_DISPONIVEIS IS 'Campos disponíveis separados por vírgula (ex: ID,NOME,EMAIL)';
COMMENT ON COLUMN TABELA_DINAMICA.CHAVE_PK IS 'Nome da coluna de chave primária';
COMMENT ON COLUMN TABELA_DINAMICA.VINCULO_ENTRE_TABELA IS 'Vínculos no formato: TabelaDestino:CampoFK:CampoPK (ex: PEDIDOS:ID_CLIENTE:ID)';
COMMENT ON COLUMN TABELA_DINAMICA.DESCRICAO_TABELA IS 'Descrição amigável da tabela';
COMMENT ON COLUMN TABELA_DINAMICA.DESCRICAO_CAMPOS IS 'Descrição dos campos no formato: Campo:Descrição|Campo:Descrição';
COMMENT ON COLUMN TABELA_DINAMICA.VISIVEL_PARA_IA IS 'Indica se a tabela é visível para consultas da IA (1=Sim, 0=Não)';
COMMENT ON COLUMN TABELA_DINAMICA.DATA_CRIACAO IS 'Data de criação do registro';
COMMENT ON COLUMN TABELA_DINAMICA.DATA_ATUALIZACAO IS 'Data da última atualização';
COMMENT ON COLUMN TABELA_DINAMICA.ATIVO IS 'Indica se o metadado está ativo (1=Sim, 0=Não)';

-- ============================================
-- 3. CRIAR ÍNDICES
-- ============================================

CREATE INDEX IDX_TABELA_DINAMICA_TABELA ON TABELA_DINAMICA(UPPER(TABELA));
CREATE INDEX IDX_TABELA_DINAMICA_ATIVO ON TABELA_DINAMICA(ATIVO);
CREATE INDEX IDX_TABELA_DINAMICA_VISIVEL ON TABELA_DINAMICA(VISIVEL_PARA_IA);

-- ============================================
-- 4. INSERIR DADOS DE EXEMPLO
-- ============================================

-- Sistema de E-commerce: CLIENTES → PEDIDOS → ITENS → PRODUTOS

INSERT INTO TABELA_DINAMICA (
    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK, VINCULO_ENTRE_TABELA,
    DESCRICAO_TABELA, DESCRICAO_CAMPOS, VISIVEL_PARA_IA
) VALUES (
    'CLIENTES',
    'ID,NOME,EMAIL,TELEFONE,CPF,DATA_CADASTRO,ATIVO',
    'ID',
    'PEDIDOS:ID_CLIENTE:ID;ENDERECOS:ID_CLIENTE:ID',
    'Cadastro de clientes do sistema',
    'ID:Identificador único|NOME:Nome completo do cliente|EMAIL:E-mail para contato|TELEFONE:Telefone principal|CPF:CPF do cliente|DATA_CADASTRO:Data de registro no sistema|ATIVO:Cliente ativo (1) ou inativo (0)',
    1
);

INSERT INTO TABELA_DINAMICA (
    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK, VINCULO_ENTRE_TABELA,
    DESCRICAO_TABELA, DESCRICAO_CAMPOS, VISIVEL_PARA_IA
) VALUES (
    'PEDIDOS',
    'ID,NUMERO,ID_CLIENTE,DATA_PEDIDO,VALOR_TOTAL,STATUS,OBSERVACOES',
    'ID',
    'CLIENTES:ID:ID_CLIENTE;ITENS_PEDIDO:ID_PEDIDO:ID',
    'Pedidos realizados pelos clientes',
    'ID:Identificador único|NUMERO:Número do pedido|ID_CLIENTE:Referência ao cliente|DATA_PEDIDO:Data de realização|VALOR_TOTAL:Valor total do pedido|STATUS:Status atual (PENDENTE,PAGO,ENVIADO,ENTREGUE,CANCELADO)|OBSERVACOES:Observações adicionais',
    1
);

INSERT INTO TABELA_DINAMICA (
    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK, VINCULO_ENTRE_TABELA,
    DESCRICAO_TABELA, DESCRICAO_CAMPOS, VISIVEL_PARA_IA
) VALUES (
    'PRODUTOS',
    'ID,CODIGO,NOME,DESCRICAO,PRECO,ESTOQUE,ID_CATEGORIA,ATIVO',
    'ID',
    'ITENS_PEDIDO:ID_PRODUTO:ID;CATEGORIAS:ID:ID_CATEGORIA',
    'Catálogo de produtos disponíveis para venda',
    'ID:Identificador único|CODIGO:Código SKU do produto|NOME:Nome do produto|DESCRICAO:Descrição detalhada|PRECO:Preço unitário|ESTOQUE:Quantidade em estoque|ID_CATEGORIA:Categoria do produto|ATIVO:Produto ativo (1) ou inativo (0)',
    1
);

INSERT INTO TABELA_DINAMICA (
    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK, VINCULO_ENTRE_TABELA,
    DESCRICAO_TABELA, DESCRICAO_CAMPOS, VISIVEL_PARA_IA
) VALUES (
    'ITENS_PEDIDO',
    'ID,ID_PEDIDO,ID_PRODUTO,QUANTIDADE,VALOR_UNITARIO,DESCONTO',
    'ID',
    'PEDIDOS:ID:ID_PEDIDO;PRODUTOS:ID:ID_PRODUTO',
    'Itens que compõem cada pedido',
    'ID:Identificador único|ID_PEDIDO:Referência ao pedido|ID_PRODUTO:Referência ao produto|QUANTIDADE:Quantidade do produto|VALOR_UNITARIO:Valor unitário no momento da compra|DESCONTO:Desconto aplicado ao item',
    1
);

INSERT INTO TABELA_DINAMICA (
    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK, VINCULO_ENTRE_TABELA,
    DESCRICAO_TABELA, DESCRICAO_CAMPOS, VISIVEL_PARA_IA
) VALUES (
    'CATEGORIAS',
    'ID,NOME,DESCRICAO,ATIVO',
    'ID',
    'PRODUTOS:ID_CATEGORIA:ID',
    'Categorias de produtos',
    'ID:Identificador único|NOME:Nome da categoria|DESCRICAO:Descrição da categoria|ATIVO:Categoria ativa (1) ou inativa (0)',
    1
);

INSERT INTO TABELA_DINAMICA (
    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK, VINCULO_ENTRE_TABELA,
    DESCRICAO_TABELA, DESCRICAO_CAMPOS, VISIVEL_PARA_IA
) VALUES (
    'ENDERECOS',
    'ID,ID_CLIENTE,RUA,NUMERO,COMPLEMENTO,BAIRRO,CIDADE,ESTADO,CEP,TIPO',
    'ID',
    'CLIENTES:ID:ID_CLIENTE',
    'Endereços dos clientes',
    'ID:Identificador único|ID_CLIENTE:Referência ao cliente|RUA:Nome da rua|NUMERO:Número|COMPLEMENTO:Complemento|BAIRRO:Bairro|CIDADE:Cidade|ESTADO:UF do estado|CEP:CEP|TIPO:Tipo de endereço (RESIDENCIAL,COMERCIAL,ENTREGA)',
    1
);

COMMIT;

-- ============================================
-- 5. CONSULTAS DE VERIFICAÇÃO
-- ============================================


-- Ver detalhes completos
SELECT * FROM TABELA_DINAMICA;


