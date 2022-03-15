using HotChocolate;
using HotChocolate.Types;

namespace Application.Api.GraphQL;

[UnionType("Event")]
public abstract record TransactionResultEvent;

public record Transferred(
    ulong Amount,
    Address From,
    Address To) : TransactionResultEvent;

public record AccountCreated(
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

/// <summary>
/// The public balance of the account was increased via a transfer from
/// encrypted to public balance.
/// </summary>
public record AmountAddedByDecryption(
    ulong Amount,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record BakerAdded(
    ulong StakedAmount,
    bool RestakeEarnings,
    ulong BakerId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    string SignKey,
    string ElectionKey,
    string AggregationKey) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record BakerKeysUpdated(
    ulong BakerId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    string SignKey,
    string ElectionKey,
    string AggregationKey) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record BakerRemoved(
    ulong BakerId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record BakerSetRestakeEarnings(
    ulong BakerId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    bool RestakeEarnings) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record BakerStakeDecreased(
    ulong BakerId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    ulong NewStakedAmount) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record BakerStakeIncreased(
    ulong BakerId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    ulong NewStakedAmount) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

/// <summary>
/// A new smart contract instance was created.
/// </summary>
/// <param name="ModuleRef">Module with the source code of the contract.</param>
/// <param name="ContractAddress">The newly assigned address of the contract.</param>
/// <param name="Amount">The amount the instance was initialized with.</param>
/// <param name="InitName">The name of the contract.</param>
/// <param name="EventsAsHex">Any contract events that might have been generated by the contract initialization.</param>
public record ContractInitialized(
    string ModuleRef,
    ContractAddress ContractAddress,
    ulong Amount,
    string InitName,
    [property: UsePaging(InferConnectionNameFromField = false)]
    string[] EventsAsHex) : TransactionResultEvent;

/// <summary>
/// A smart contract module was successfully deployed.
/// </summary>
/// <param name="ModuleRef"></param>
public record ContractModuleDeployed(
    string ModuleRef) : TransactionResultEvent;

/// <summary>
/// A smart contract instance was updated.
/// </summary>
/// <param name="ContractAddress">Address of the affected instance.</param>
/// <param name="Instigator">The origin of the message to the smart contract. This can be either an account or a smart contract.</param>
/// <param name="Amount">The amount the method was invoked with.</param>
/// <param name="MessageAsHex">The message passed to method.</param>
/// <param name="ReceiveName">The name of the method that was executed.</param>
/// <param name="EventsAsHex">Any contract events that might have been generated by the contract execution.</param>
public record ContractUpdated(
    ContractAddress ContractAddress,
    Address Instigator,
    ulong Amount,
    string MessageAsHex,
    string ReceiveName,
    [property: UsePaging(InferConnectionNameFromField = false)]
    string[] EventsAsHex) : TransactionResultEvent;

public record CredentialDeployed(
    string RegId,
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

/// <summary>
/// Keys of the given credential were updated.
/// </summary>
/// <param name="CredId">ID of the credential whose keys were updated.</param>
public record CredentialKeysUpdated(
    string CredId) : TransactionResultEvent;

/// <summary>
/// The credentials of the account were updated.
/// </summary>
/// <param name="AccountAddress">The affected account.</param>
/// <param name="NewCredIds">The credential ids that were added.</param>
/// <param name="RemovedCredIds">The credential ids that were removed.</param>
/// <param name="NewThreshold">The (possibly) updated account threshold.</param>
public record CredentialsUpdated(
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    string[] NewCredIds,
    string[] RemovedCredIds,
    byte NewThreshold) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

/// <summary>
/// Data was registered on the chain.
/// </summary>
/// <param name="DataAsHex">The data that was registered.</param>
public record DataRegistered(
    string DataAsHex) : TransactionResultEvent;

/// <summary>
/// Event generated when one or more encrypted amounts are consumed from the account
/// </summary>
/// <param name="AccountAddress">The affected account</param>
/// <param name="NewEncryptedAmount">The new self encrypted amount on the affected account</param>
/// <param name="InputAmount">The input encrypted amount that was removed</param>
/// <param name="UpToIndex">The index indicating which amounts were used</param>
public record EncryptedAmountsRemoved(
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    string NewEncryptedAmount,
    string InputAmount,
    ulong UpToIndex) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

/// <summary>
/// The encrypted balance of the account was updated due to transfer from
/// public to encrypted balance of the account.
/// </summary>
/// <param name="AccountAddress">The affected account</param>
/// <param name="NewEncryptedAmount">The new self encrypted amount of the account</param>
/// <param name="Amount">The amount that was transferred from public to encrypted balance</param>
public record EncryptedSelfAmountAdded(
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    string NewEncryptedAmount,
    ulong Amount) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

/// <summary>
/// A new encrypted amount was added to the account.
/// </summary>
/// <param name="AccountAddress">The account onto which the amount was added.</param>
/// <param name="NewIndex">The index the amount was assigned.</param>
/// <param name="EncryptedAmount">The encrypted amount that was added.</param>
public record NewEncryptedAmount(
    [property:GraphQLDeprecated("Use 'AccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string AccountAddress,
    ulong NewIndex,
    string EncryptedAmount) : TransactionResultEvent
{
    public string AccountAddressString => AccountAddress;
}

public record TransferMemo(string RawHex) : TransactionResultEvent
{
    public DecodedTransferMemo GetDecoded()
    {
        return DecodedTransferMemo.CreateFromHex(RawHex);
    }
}

/// <summary>
/// A transfer with schedule was enqueued.
/// </summary>
/// <param name="FromAccountAddress">Sender account address.</param>
/// <param name="ToAccountAddress">Receiver account address.</param>
/// <param name="AmountsSchedule">The list of releases. Ordered by increasing timestamp.</param>
public record TransferredWithSchedule(
    [property:GraphQLDeprecated("Use 'FromAccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string FromAccountAddress,
    [property:GraphQLDeprecated("Use 'ToAccountAddressString' instead. Type of this field will be changed to AccountAddress in the near future.")]
    string ToAccountAddress,
    [property: UsePaging] TimestampedAmount[] AmountsSchedule) : TransactionResultEvent
{
    public string FromAccountAddressString => FromAccountAddress;
    public string ToAccountAddressString => ToAccountAddress;
    public ulong TotalAmount => AmountsSchedule.Aggregate(0UL, (val, item) => val + item.Amount);
}

/// <summary>
/// A chain update was enqueued for the given time.
/// </summary>
/// <param name="EffectiveTime"></param>
public record ChainUpdateEnqueued(
    DateTimeOffset EffectiveTime) : TransactionResultEvent; // TODO: Add payload for the update - probably yet another large union!