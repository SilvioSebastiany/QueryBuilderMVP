using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using QueryBuilder.Domain.Entities;
using QueryBuilder.Domain.Interfaces;

namespace QueryBuilder.Infra.Data.Repositories
{
    /// <summary>
    /// Repositório de metadados usando Dapper
    /// </summary>
    public class MetadadosRepository : IMetadadosRepository
    {
        private readonly IDbConnection _connection;

        public MetadadosRepository(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<TabelaDinamica?> ObterPorIdAsync(int id)
        {
            const string sql = @"
                SELECT ID, TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK,
                       VINCULO_ENTRE_TABELA, DESCRICAO_TABELA,
                       DESCRICAO_CAMPOS, VISIVEL_PARA_IA,
                       DATA_CRIACAO, DATA_ATUALIZACAO, ATIVO
                FROM TABELA_DINAMICA
                WHERE ID = :Id";

            var result = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
            return result != null ? MapToEntity(result) : null;
        }

        public async Task<TabelaDinamica?> ObterPorNomeTabelaAsync(string nomeTabela)
        {
            const string sql = @"
                SELECT ID, TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK,
                       VINCULO_ENTRE_TABELA, DESCRICAO_TABELA,
                       DESCRICAO_CAMPOS, VISIVEL_PARA_IA,
                       DATA_CRIACAO, DATA_ATUALIZACAO, ATIVO
                FROM TABELA_DINAMICA
                WHERE UPPER(TABELA) = UPPER(:NomeTabela)";

            var result = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { NomeTabela = nomeTabela });
            return result != null ? MapToEntity(result) : null;
        }

        public async Task<IEnumerable<TabelaDinamica>> ObterTodosAsync(bool apenasAtivos = true)
        {
            var sql = @"
                SELECT ID, TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK,
                       VINCULO_ENTRE_TABELA, DESCRICAO_TABELA,
                       DESCRICAO_CAMPOS, VISIVEL_PARA_IA,
                       DATA_CRIACAO, DATA_ATUALIZACAO, ATIVO
                FROM TABELA_DINAMICA";

            if (apenasAtivos)
                sql += " WHERE ATIVO = 1";

            sql += " ORDER BY TABELA";

            var results = await _connection.QueryAsync<dynamic>(sql);
            return results.Select(MapToEntity);
        }

        public async Task<IEnumerable<TabelaDinamica>> ObterVisiveisParaIAAsync()
        {
            const string sql = @"
                SELECT ID, TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK,
                       VINCULO_ENTRE_TABELA, DESCRICAO_TABELA,
                       DESCRICAO_CAMPOS, VISIVEL_PARA_IA,
                       DATA_CRIACAO, DATA_ATUALIZACAO, ATIVO
                FROM TABELA_DINAMICA
                WHERE VISIVEL_PARA_IA = 1 AND ATIVO = 1
                ORDER BY TABELA";

            var results = await _connection.QueryAsync<dynamic>(sql);
            return results.Select(MapToEntity);
        }

        public async Task<int> CriarAsync(TabelaDinamica tabela)
        {
            const string sql = @"
                INSERT INTO TABELA_DINAMICA (
                    TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK,
                    VINCULO_ENTRE_TABELA, DESCRICAO_TABELA,
                    DESCRICAO_CAMPOS, VISIVEL_PARA_IA,
                    DATA_CRIACAO, ATIVO
                ) VALUES (
                    :Tabela, :CamposDisponiveis, :ChavePk,
                    :VinculoEntreTabela, :DescricaoTabela,
                    :DescricaoCampos, :VisivelParaIA,
                    :DataCriacao, :Ativo
                )
                RETURNING ID INTO :Id";

            var parameters = new DynamicParameters();
            parameters.Add("Tabela", tabela.Tabela);
            parameters.Add("CamposDisponiveis", tabela.CamposDisponiveis);
            parameters.Add("ChavePk", tabela.ChavePk);
            parameters.Add("VinculoEntreTabela", tabela.VinculoEntreTabela);
            parameters.Add("DescricaoTabela", tabela.DescricaoTabela);
            parameters.Add("DescricaoCampos", tabela.DescricaoCampos);
            parameters.Add("VisivelParaIA", tabela.VisivelParaIA ? 1 : 0);
            parameters.Add("DataCriacao", DateTime.Now);
            parameters.Add("Ativo", tabela.Ativo ? 1 : 0);
            parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters);
            return parameters.Get<int>("Id");
        }

        public async Task AtualizarAsync(TabelaDinamica tabela)
        {
            const string sql = @"
                UPDATE TABELA_DINAMICA SET
                    CAMPOS_DISPONIVEIS = :CamposDisponiveis,
                    CHAVE_PK = :ChavePk,
                    VINCULO_ENTRE_TABELA = :VinculoEntreTabela,
                    DESCRICAO_TABELA = :DescricaoTabela,
                    DESCRICAO_CAMPOS = :DescricaoCampos,
                    VISIVEL_PARA_IA = :VisivelParaIA,
                    DATA_ATUALIZACAO = :DataAtualizacao,
                    ATIVO = :Ativo
                WHERE ID = :Id";

            await _connection.ExecuteAsync(sql, new
            {
                Id = tabela.Id,
                CamposDisponiveis = tabela.CamposDisponiveis,
                ChavePk = tabela.ChavePk,
                VinculoEntreTabela = tabela.VinculoEntreTabela,
                DescricaoTabela = tabela.DescricaoTabela,
                DescricaoCampos = tabela.DescricaoCampos,
                VisivelParaIA = tabela.VisivelParaIA ? 1 : 0,
                DataAtualizacao = DateTime.Now,
                Ativo = tabela.Ativo ? 1 : 0
            });
        }

        public async Task DeletarAsync(int id)
        {
            const string sql = "DELETE FROM TABELA_DINAMICA WHERE ID = :Id";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<bool> ExisteAsync(string nomeTabela)
        {
            const string sql = "SELECT COUNT(*) FROM TABELA_DINAMICA WHERE UPPER(TABELA) = UPPER(:NomeTabela)";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { NomeTabela = nomeTabela });
            return count > 0;
        }

        public async Task<IEnumerable<TabelaDinamica>> ObterPorVinculoAsync(string nomeTabela)
        {
            const string sql = @"
                SELECT ID, TABELA, CAMPOS_DISPONIVEIS, CHAVE_PK,
                       VINCULO_ENTRE_TABELA, DESCRICAO_TABELA,
                       DESCRICAO_CAMPOS, VISIVEL_PARA_IA,
                       DATA_CRIACAO, DATA_ATUALIZACAO, ATIVO
                FROM TABELA_DINAMICA
                WHERE VINCULO_ENTRE_TABELA LIKE :Pattern AND ATIVO = 1";

            var results = await _connection.QueryAsync<dynamic>(sql, new { Pattern = $"%{nomeTabela}.%" });
            return results.Select(MapToEntity);
        }

        // Helper para mapear dynamic para Entity (usando reflection limitado)
        private TabelaDinamica MapToEntity(dynamic row)
        {
            // Como o construtor é privado, usamos reflection para criar a instância
            var entity = TabelaDinamica.Criar(
                tabela: row.TABELA,
                camposDisponiveis: row.CAMPOS_DISPONIVEIS,
                chavePk: row.CHAVE_PK,
                vinculoEntreTabela: row.VINCULO_ENTRE_TABELA,
                descricaoTabela: row.DESCRICAO_TABELA,
                descricaoCampos: row.DESCRICAO_CAMPOS,
                visivelParaIA: Convert.ToBoolean(row.VISIVEL_PARA_IA)
            );

            // Usar reflection para setar o ID (propriedade privada)
            var idProp = typeof(TabelaDinamica).GetProperty("Id");
            idProp?.SetValue(entity, Convert.ToInt32(row.ID));

            return entity;
        }
    }
}
