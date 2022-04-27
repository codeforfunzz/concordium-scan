﻿using Application.Api.GraphQL.Bakers;
using Application.Api.GraphQL.EfCore.Converters.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Api.GraphQL.EfCore.EntityTypeConfigurations;

public class BakerRewardConfiguration : IEntityTypeConfiguration<BakerReward>
{
    public void Configure(EntityTypeBuilder<BakerReward> builder)
    {
        builder.ToView("graphql_baker_rewards")
            .HasNoKey();
        
        builder.Property(x => x.BakerId).HasColumnName("baker_id");
        builder.Property(x => x.Index).HasColumnName("index").ValueGeneratedOnAdd();
        builder.Property(x => x.Timestamp).HasColumnName("time").HasConversion<DateTimeOffsetToTimestampConverter>();
        builder.Property(x => x.RewardType).HasColumnName("reward_type"); 
        builder.Property(x => x.Amount).HasColumnName("amount");
        builder.Property(x => x.BlockId).HasColumnName("block_id");
    }
}