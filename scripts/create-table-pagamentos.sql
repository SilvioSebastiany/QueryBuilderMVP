-- =====================================================
-- Script: Criação da Tabela PAGAMENTOS
-- Descrição: Tabela para controle de pagamentos de pedidos
-- Data: 2025-11-20
-- =====================================================

-- Criar tabela PAGAMENTOS
create table pagamentos (
   id              number generated always as identity primary key,
   id_pedido       number not null,
   forma_pagamento varchar2(50) not null,
   valor           number(10,2) not null,
   data_pagamento  date not null,
   status          varchar2(20) default 'PENDENTE' not null,
   numero_parcelas number(2) default 1,
   parcela_atual   number(2) default 1,
   data_vencimento date,
   observacoes     varchar2(500),
   constraint fk_pagamentos_pedido foreign key ( id_pedido )
      references pedidos ( id ),
   constraint chk_status_pagamento
      check ( status in ( 'PENDENTE',
                          'APROVADO',
                          'RECUSADO',
                          'ESTORNADO' ) ),
   constraint chk_forma_pagamento
      check ( forma_pagamento in ( 'CREDITO',
                                   'DEBITO',
                                   'PIX',
                                   'BOLETO',
                                   'DINHEIRO' ) )
);

-- Criar índice para melhorar performance
create index idx_pagamentos_pedido on
   pagamentos (
      id_pedido
   );
create index idx_pagamentos_status on
   pagamentos (
      status
   );

-- Comentários nas colunas
comment on table pagamentos is
   'Tabela de controle de pagamentos dos pedidos';
comment on column pagamentos.id is
   'Identificador único do pagamento';
comment on column pagamentos.id_pedido is
   'Referência para o pedido';
comment on column pagamentos.forma_pagamento is
   'Forma de pagamento utilizada';
comment on column pagamentos.valor is
   'Valor do pagamento';
comment on column pagamentos.data_pagamento is
   'Data em que o pagamento foi realizado';
comment on column pagamentos.status is
   'Status do pagamento (PENDENTE, APROVADO, RECUSADO, ESTORNADO)';
comment on column pagamentos.numero_parcelas is
   'Número total de parcelas';
comment on column pagamentos.parcela_atual is
   'Número da parcela atual';
comment on column pagamentos.data_vencimento is
   'Data de vencimento do pagamento';
comment on column pagamentos.observacoes is
   'Observações sobre o pagamento';

-- =====================================================
-- Inserts de dados de exemplo
-- =====================================================

-- Pagamentos do Pedido 1 (Cliente: João Silva)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   data_vencimento,
   observacoes
) values ( 1,
           'CREDITO',
           1299.99,
           to_date('2024-01-15','YYYY-MM-DD'),
           'APROVADO',
           3,
           1,
           to_date('2024-02-15','YYYY-MM-DD'),
           'Primeira parcela de 3x' );

insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   data_vencimento,
   observacoes
) values ( 1,
           'CREDITO',
           1299.99,
           to_date('2024-02-15','YYYY-MM-DD'),
           'APROVADO',
           3,
           2,
           to_date('2024-03-15','YYYY-MM-DD'),
           'Segunda parcela de 3x' );

insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   data_vencimento,
   observacoes
) values ( 1,
           'CREDITO',
           1299.99,
           to_date('2024-03-15','YYYY-MM-DD'),
           'PENDENTE',
           3,
           3,
           to_date('2024-04-15','YYYY-MM-DD'),
           'Terceira parcela de 3x' );

-- Pagamento do Pedido 2 (Cliente: Maria Santos)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   observacoes
) values ( 2,
           'PIX',
           85.00,
           to_date('2024-02-10','YYYY-MM-DD'),
           'APROVADO',
           1,
           1,
           'Pagamento à vista via PIX' );

-- Pagamento do Pedido 3 (Cliente: Carlos Oliveira)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   data_vencimento,
   observacoes
) values ( 3,
           'BOLETO',
           2199.98,
           to_date('2024-03-05','YYYY-MM-DD'),
           'PENDENTE',
           1,
           1,
           to_date('2024-03-15','YYYY-MM-DD'),
           'Aguardando confirmação bancária' );

-- Pagamento do Pedido 4 (Cliente: Ana Costa)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   observacoes
) values ( 4,
           'DEBITO',
           699.99,
           to_date('2024-04-01','YYYY-MM-DD'),
           'APROVADO',
           1,
           1,
           'Pagamento em débito aprovado' );

-- Pagamento do Pedido 5 (Cliente: Pedro Alves)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   observacoes
) values ( 5,
           'DINHEIRO',
           149.90,
           to_date('2024-05-10','YYYY-MM-DD'),
           'APROVADO',
           1,
           1,
           'Pagamento em espécie na entrega' );

-- Pagamento do Pedido 6 (Cliente: Lucia Ferreira)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   data_vencimento,
   observacoes
) values ( 6,
           'CREDITO',
           1249.95,
           to_date('2024-06-20','YYYY-MM-DD'),
           'APROVADO',
           2,
           1,
           to_date('2024-07-20','YYYY-MM-DD'),
           'Primeira de 2 parcelas' );

insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   data_vencimento,
   observacoes
) values ( 6,
           'CREDITO',
           1249.95,
           to_date('2024-07-20','YYYY-MM-DD'),
           'PENDENTE',
           2,
           2,
           to_date('2024-08-20','YYYY-MM-DD'),
           'Segunda de 2 parcelas' );

-- Pagamento RECUSADO (exemplo)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   observacoes
) values ( 1,
           'CREDITO',
           1299.99,
           to_date('2024-01-10','YYYY-MM-DD'),
           'RECUSADO',
           1,
           1,
           'Cartão sem limite - tentativa anterior' );

-- Pagamento ESTORNADO (exemplo)
insert into pagamentos (
   id_pedido,
   forma_pagamento,
   valor,
   data_pagamento,
   status,
   numero_parcelas,
   parcela_atual,
   observacoes
) values ( 2,
           'PIX',
           85.00,
           to_date('2024-02-09','YYYY-MM-DD'),
           'ESTORNADO',
           1,
           1,
           'Cliente solicitou cancelamento' );

commit;

-- =====================================================
-- Insert na tabela TABELA_DINAMICA
-- =====================================================

insert into tabela_dinamica (
   tabela,
   campos_disponiveis,
   chave_pk,
   vinculo_entre_tabela,
   descricao_tabela,
   descricao_campos,
   visivel_para_ia,
   data_criacao,
   ativo
) values ( 'PAGAMENTOS',
           'ID,ID_PEDIDO,FORMA_PAGAMENTO,VALOR,DATA_PAGAMENTO,STATUS,NUMERO_PARCELAS,PARCELA_ATUAL,DATA_VENCIMENTO,OBSERVACOES'
           ,
           'ID',
           'PEDIDOS:ID_PEDIDO:ID',
           'Tabela de controle de pagamentos dos pedidos do e-commerce',
           'ID: Identificador único do pagamento; ID_PEDIDO: Referência ao pedido; FORMA_PAGAMENTO: Tipo de pagamento (CREDITO, DEBITO, PIX, BOLETO, DINHEIRO); VALOR: Valor pago; DATA_PAGAMENTO: Data do pagamento; STATUS: Status do pagamento (PENDENTE, APROVADO, RECUSADO, ESTORNADO); NUMERO_PARCELAS: Quantidade total de parcelas; PARCELA_ATUAL: Número da parcela; DATA_VENCIMENTO: Vencimento da parcela; OBSERVACOES: Informações adicionais'
           ,
           1,
           sysdate,
           1 );

commit;

-- =====================================================
-- Consulta de verificação
-- =====================================================

select *
  from pagamentos
 order by id;
select *
  from tabela_dinamica
 where tabela = 'PAGAMENTOS';

-- Consulta com JOIN para verificar relacionamento
select pag.id,
       pag.forma_pagamento,
       pag.valor,
       pag.status,
       ped.numero as pedido_numero,
       cli.nome as cliente_nome
  from pagamentos pag
 inner join pedidos ped
on pag.id_pedido = ped.id
 inner join clientes cli
on ped.id_cliente = cli.id
 order by pag.id;
