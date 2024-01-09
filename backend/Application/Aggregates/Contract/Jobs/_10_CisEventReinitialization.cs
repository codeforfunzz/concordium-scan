using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Application.Aggregates.Contract.Configurations;
using Application.Aggregates.Contract.Dto;
using Application.Aggregates.Contract.Entities;
using Application.Aggregates.Contract.EventLogs;
using Application.Api.GraphQL;
using Application.Api.GraphQL.EfCore;
using Application.Api.GraphQL.Transactions;
using Application.Resilience;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Aggregates.Contract.Jobs;

/// <summary>
/// The job starts by truncate tables graphql_account_tokens, graphql_token_events and graphql_tokens. Reinitialization
/// is needed both because mint events was not correctly updating account token balances.
///
/// For each contract those contract actions, which generates log events, are processed
/// (contract initialization, contract interrupted and contract updated).
///
/// Each log events is checked if it should be parsed, see <see cref="CisEvent"/>. If the contract has a linked
/// schema and a successfully human interpretable log event linked, the human interpretable log event is linked to
/// the event. This may contain additional data.
/// </summary>
public sealed class _10_CisEventReinitialization : IStatelessJob
{
    private readonly IDbContextFactory<GraphQlDbContext> _contextFactory;
    private readonly IEventLogHandler _eventLogHandler;
    private readonly ContractAggregateOptions _options;
    private readonly ILogger _logger;

    /// <summary>
    /// WARNING - Do not change this if job already executed on environment, since it will trigger rerun of job.
    /// </summary>
    private const string JobName = "_10_CisEventReinitialization";

    public _10_CisEventReinitialization(
        IDbContextFactory<GraphQlDbContext> contextFactory,
        IEventLogHandler eventLogHandler,
        IOptions<ContractAggregateOptions> options)
    {
        _contextFactory = contextFactory;
        _eventLogHandler = eventLogHandler;
        _options = options.Value;
        _logger = Log.ForContext<_10_CisEventReinitialization>();
    }

    /// <inheritdoc/>
    public string GetUniqueIdentifier() => JobName;

    /// <inheritdoc/>
    public async Task<IEnumerable<int>> GetIdentifierSequence(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var max = context.ContractEvents.Max(ce => ce.ContractAddressIndex);
        var shift = 4000;
        return Enumerable.Range(shift, (int)max + 1 - shift);
    }

    /// <inheritdoc/>
    public async ValueTask Setup(CancellationToken token)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(token);
        var connection = context.Database.GetDbConnection();
        await connection.ExecuteAsync("truncate table graphql_account_tokens, graphql_token_events, graphql_tokens;");
    }

    /// <inheritdoc/>
    public async ValueTask Process(int identifier, CancellationToken token = default)
    {
        _logger.Debug($"Start processing {identifier}");
        await Policies.GetTransientPolicy(GetUniqueIdentifier(), _logger, _options.RetryCount, _options.RetryDelay)
            .ExecuteAsync(async () =>
            {
                TransactionScope? transactionScope = null;
                try
                {
                    transactionScope = new TransactionScope(
                        TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                        TransactionScopeAsyncFlowOption.Enabled);

                    var contractEvents = await GetContractEvents(identifier, token);
                    var jobContractRepository = new JobContractRepository(contractEvents);
                    await _eventLogHandler.HandleCisEvent(jobContractRepository);
                    transactionScope.Complete();
                    _logger.Debug($"Completed successfully processing {identifier}");
                }
                catch (Exception e)
                {
                    _logger.Warning(e, $"Exception on identifier {identifier}");
                    throw;
                }
                finally
                {
                    transactionScope?.Dispose();
                }
            });
    }

    /// <inheritdoc/>
    public bool ShouldNodeImportAwait() => true;

    private async Task<IList<ContractEvent>> GetContractEvents(int address, CancellationToken token = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(token);
        var contractEvents = await context.ContractEvents.Where(ce => ce.ContractAddressIndex == (ulong)address)
            .ToListAsync(cancellationToken: token);
        return contractEvents;
    }
    
    private sealed class JobContractRepository : IContractRepository
    {
        private readonly IEnumerable<ContractEvent> _contractEvents;
        
        public JobContractRepository(IEnumerable<ContractEvent> contractEvents)
        {
            _contractEvents = contractEvents;
        }
    
        public IEnumerable<ContractEvent> GetContractEventsAddedInTransaction() => _contractEvents;
    
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    
        public Task<IList<TransactionRejectEventDto>> FromBlockHeightRangeGetContractRelatedRejections(ulong heightFrom, ulong heightTo)
        {
            throw new NotImplementedException();
        }
    
        public Task<IList<TransactionResultEventDto>> FromBlockHeightRangeGetContractRelatedTransactionResultEventRelations(ulong heightFrom, ulong heightTo)
        {
            throw new NotImplementedException();
        }
    
        public Task<List<ulong>> FromBlockHeightRangeGetBlockHeightsReadOrdered(ulong heightFrom, ulong heightTo)
        {
            throw new NotImplementedException();
        }
    
        public Task<ContractReadHeight?> GetReadonlyLatestContractReadHeight()
        {
            throw new NotImplementedException();
        }
    
        public Task<long> GetReadonlyLatestImportState(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    
        public Task<ContractInitialized> GetReadonlyContractInitializedEventAsync(ContractAddress contractAddress)
        {
            throw new NotImplementedException();
        }
    
        public Task AddAsync<T>(params T[] entities) where T : class
        {
            throw new NotImplementedException();
        }
    
        public Task AddRangeAsync<T>(IEnumerable<T> heights) where T : class
        {
            throw new NotImplementedException();
        }
    
        public Task CommitAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
