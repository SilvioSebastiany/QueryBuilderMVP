using MediatR;
using Microsoft.Extensions.Logging;
using QueryBuilder.Domain.Commands.Metadados;
using QueryBuilder.Domain.Interfaces;
using QueryBuilder.Domain.Notifications;

namespace QueryBuilder.Domain.Commands.Handlers
{
    /// <summary>
    /// Handler para atualização de metadados de tabela dinâmica
    /// </summary>
    public class AtualizarMetadadoCommandHandler : IRequestHandler<AtualizarMetadadoCommand, bool>
    {
        private readonly IMetadadosRepository _repository;
        private readonly INotificationContext _notificationContext;
        private readonly ILogger<AtualizarMetadadoCommandHandler> _logger;

        public AtualizarMetadadoCommandHandler(
            IMetadadosRepository repository,
            INotificationContext notificationContext,
            ILogger<AtualizarMetadadoCommandHandler> logger)
        {
            _repository = repository;
            _notificationContext = notificationContext;
            _logger = logger;
        }

        public async Task<bool> Handle(AtualizarMetadadoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Atualizando metadado ID: {Id}", request.Id);

            // Buscar metadado existente
            var metadado = await _repository.ObterPorIdAsync(request.Id);
            if (metadado == null)
            {
                _notificationContext.AddNotification(
                    "NotFound",
                    $"Metadado com ID {request.Id} não encontrado");
                
                _logger.LogWarning("Tentativa de atualizar metadado inexistente - ID: {Id}", request.Id);
                return false;
            }

            try
            {
                // Atualizar entidade de domínio usando métodos de comportamento
                metadado.AtualizarCampos(request.CamposDisponiveis);
                
                if (!string.IsNullOrWhiteSpace(request.VinculoEntreTabela))
                {
                    metadado.AtualizarVinculo(request.VinculoEntreTabela);
                }

                if (!string.IsNullOrWhiteSpace(request.DescricaoTabela) || !string.IsNullOrWhiteSpace(request.DescricaoCampos))
                {
                    metadado.AtualizarDescricao(request.DescricaoTabela, request.DescricaoCampos);
                }

                metadado.AlterarVisibilidadeIA(request.VisivelParaIA);

                // Persistir no banco
                await _repository.AtualizarAsync(metadado);

                _logger.LogInformation(
                    "Metadado atualizado com sucesso - ID: {Id}, Tabela: {Tabela}",
                    metadado.Id, metadado.Tabela);

                return true;
            }
            catch (ArgumentException ex)
            {
                // Exceções de validação do domínio viram notificações
                _notificationContext.AddNotification("Validacao", ex.Message);
                _logger.LogWarning(ex, "Erro de validação ao atualizar metadado ID: {Id}", request.Id);
                return false;
            }
            catch (Exception ex)
            {
                _notificationContext.AddNotification("Erro", "Erro ao atualizar metadado no banco de dados");
                _logger.LogError(ex, "Erro ao atualizar metadado ID: {Id}", request.Id);
                return false;
            }
        }
    }
}
