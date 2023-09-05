using Application.Aggregates.Contract.BackgroundServices;
using Application.Aggregates.Contract.Configurations;
using Application.Aggregates.Contract.Jobs;
using Application.Aggregates.Contract.Observability;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Application.Aggregates.Contract.Extensions;

public static class ContractExtensions
{
    public static void AddContractAggregate(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.Configure<ContractAggregate>(configuration.GetSection("ContractAggregate"));

        collection.AddHostedService<ContractNodeImportBackgroundService>();
        
        collection.AddTransient<IContractRepositoryFactory, ContractRepositoryFactory>();
        collection.AddTransient<IContractNodeClient, ContractNodeClient>();
        
        collection.AddContractJobs();
        collection.AddObservability();
        
        AddDapperTypeHandlers();
    }

    private static void AddObservability(this IServiceCollection collection)
    {
        collection.AddSingleton<ContractHealthCheck>();
        collection.AddHealthChecks()
            .AddCheck<ContractHealthCheck>("Contract", HealthStatus.Unhealthy)
            .ForwardToPrometheus();
    }
    
    /// <summary>
    /// Used by <see cref="Dapper"/> to specify custom mappings of types.
    /// </summary>
    internal static void AddDapperTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new TransactionResultEventHandler());
        SqlMapper.AddTypeHandler(new TransactionTypeUnionHandler());
        SqlMapper.AddTypeHandler(new AccountAddressHandler());
    }

    /// <summary>
    /// Background service which executes all jobs related to Smart Contracts.
    ///
    /// When new is implemented they should be added to the <see cref="ContractJobsBackgroundService"/>.
    /// </summary>
    private static void AddContractJobs(this IServiceCollection collection)
    {
        collection.AddHostedService<ContractJobsBackgroundService>();
        collection.AddTransient<IContractJobFinder, ContractJobFinder>();

        collection.AddSingleton<IContractJobRepository, ContractJobRepository>();
        collection.AddTransient<IContractJob, ContractDatabaseImportJob>();
    }
}
