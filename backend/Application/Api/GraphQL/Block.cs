﻿using Application.Api.GraphQL.EfCore;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;

namespace Application.Api.GraphQL;

public class Block
{
    [ID]
    public long Id { get; set; }
    public string BlockHash { get; set; }
    public int BlockHeight { get; set; }
    public DateTimeOffset BlockSlotTime { get; init; }
    public int? BakerId { get; init; }
    public bool Finalized { get; init; }
    public int TransactionCount { get; init; }
    public SpecialEvents SpecialEvents { get; init; }
    public FinalizationSummary? FinalizationSummary { get; init; }
    
    public BalanceStatistics BalanceStatistics { get; init; }

    [UseDbContext(typeof(GraphQlDbContext))]
    [UsePaging]
    public IEnumerable<Transaction> GetTransactions([ScopedService] GraphQlDbContext dbContext)
    {
        return dbContext.Transactions
            .AsNoTracking()
            .Where(tx => tx.BlockId == Id).OrderBy(x => x.TransactionIndex);
    }
}