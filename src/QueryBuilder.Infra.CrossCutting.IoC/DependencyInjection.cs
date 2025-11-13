using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using QueryBuilder.Domain.Interfaces;
using QueryBuilder.Infra.CrossCutting.Settings;
using QueryBuilder.Infra.Data.Repositories;

namespace QueryBuilder.Infra.CrossCutting.IoC
{
    /// <summary>
    /// Configuração de Dependency Injection
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Settings
            var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>()
                ?? throw new InvalidOperationException("DatabaseSettings não configurado");

            services.AddSingleton(databaseSettings);

            // Database Connection
            services.AddScoped<IDbConnection>(provider =>
            {
                var connection = new OracleConnection(databaseSettings.ConnectionString);
                connection.Open();
                return connection;
            });

            // Repositories
            services.AddScoped<IMetadadosRepository, MetadadosRepository>();

            return services;
        }
    }
}
