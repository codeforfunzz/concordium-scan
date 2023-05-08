﻿using System.Text.Json;
using Application.Api.GraphQL.EfCore.Converters.Json;

namespace Application.Api.GraphQL.EfCore.Converters.EfCore;

public static class EfCoreJsonSerializerOptionsFactory 
{
    public static JsonSerializerOptions Create()
    {
        return new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true,
            Converters =
            {
                new TransactionRejectReasonConverter(),
                new TransactionResultEventConverter(),
                new TokenTransactionDataConverter(),
                new AddressConverter(),
                new Json.AccountAddressConverter(),
                new ContractAddressConverter(),
                new ChainUpdatePayloadConverter(),
                new PendingBakerChangeConverter(),
                new PendingDelegationChangeConverter(),
                new DelegationTargetConverter()
            }
        };
    }
}