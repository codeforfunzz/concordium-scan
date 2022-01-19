using HotChocolate.Types;

namespace Application.Api.GraphQL;

[UnionType("Event")]
public abstract record TransactionResultEvent;

public record Transferred(
    ulong Amount,
    Address From,
    Address To) : TransactionResultEvent;

public record AccountCreated(
    string Address) : TransactionResultEvent;

/// <summary>
/// The public balance of the account was increased via a transfer from
/// encrypted to public balance.
/// </summary>
public record AmountAddedByDecryption(
    ulong Amount,
    string AccountAddress) : TransactionResultEvent;

public record BakerAdded(
    ulong StakedAmount,
    bool RestakeEarnings,
    ulong BakerId,
    string AccountAddress,
    string SignKey,
    string ElectionKey,
    string AggregationKey) : TransactionResultEvent;

public record BakerKeysUpdated(
    ulong BakerId,
    string AccountAddress,
    string SignKey,
    string ElectionKey,
    string AggregationKey) : TransactionResultEvent;

public record BakerRemoved(
    ulong BakerId,
    string AccountAddress) : TransactionResultEvent;

public record BakerSetRestakeEarnings(
    ulong BakerId,
    string AccountAddress,
    bool RestakeEarnings) : TransactionResultEvent;

public record BakerStakeDecreased(
    ulong BakerId,
    string AccountAddress,
    ulong NewStakedAmount) : TransactionResultEvent;

public record BakerStakeIncreased(
    ulong BakerId,
    string AccountAddress,
    ulong NewStakedAmount) : TransactionResultEvent;

/// <summary>
/// A new smart contract instance was created.
/// </summary>
/// <param name="ModuleRef">Module with the source code of the contract.</param>
/// <param name="Address">The newly assigned address of the contract.</param>
/// <param name="Amount">The amount the instance was initialized with.</param>
/// <param name="InitName">The name of the contract.</param>
/// <param name="EventsAsHex">Any contract events that might have been generated by the contract initialization.</param>
public record ContractInitialized(
    string ModuleRef,
    ContractAddress Address,
    ulong Amount,
    string InitName,
    [property:UsePaging(InferConnectionNameFromField = false)]
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
/// <param name="Address">Address of the affected instance.</param>
/// <param name="Instigator">The origin of the message to the smart contract. This can be either an account or a smart contract.</param>
/// <param name="Amount">The amount the method was invoked with.</param>
/// <param name="MessageAsHex">The message passed to method.</param>
/// <param name="ReceiveName">The name of the method that was executed.</param>
/// <param name="EventsAsHex">Any contract events that might have been generated by the contract execution.</param>
public record ContractUpdated(
    ContractAddress Address,
    Address Instigator,
    ulong Amount,
    string MessageAsHex, 
    string ReceiveName,
    [property:UsePaging(InferConnectionNameFromField = false)]
    string[] EventsAsHex) : TransactionResultEvent;

public record CredentialDeployed(
    string RegId,
    string AccountAddress) : TransactionResultEvent;

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
    string AccountAddress,
    string[] NewCredIds,
    string[] RemovedCredIds,
    byte NewThreshold) : TransactionResultEvent;

public record DataRegistered(
    string Data) : TransactionResultEvent; //TODO: how should we represent binary data on graphql?

/// <summary>
/// Event generated when one or more encrypted amounts are consumed from the account
/// </summary>
/// <param name="AccountAddress">The affected account</param>
/// <param name="NewEncryptedAmount">The new self encrypted amount on the affected account</param>
/// <param name="InputAmount">The input encrypted amount that was removed</param>
/// <param name="UpToIndex">The index indicating which amounts were used</param>
public record EncryptedAmountsRemoved(
    string AccountAddress,
    string NewEncryptedAmount,
    string InputAmount,
    ulong UpToIndex) : TransactionResultEvent;

/// <summary>
/// The encrypted balance of the account was updated due to transfer from
/// public to encrypted balance of the account.
/// </summary>
/// <param name="AccountAddress">The affected account</param>
/// <param name="NewEncryptedAmount">The new self encrypted amount of the account</param>
/// <param name="Amount">The amount that was transferred from public to encrypted balance</param>
public record EncryptedSelfAmountAdded(
    string AccountAddress,
    string NewEncryptedAmount,
    ulong Amount) : TransactionResultEvent;

/// <summary>
/// A new encrypted amount was added to the account.
/// </summary>
/// <param name="AccountAddress">The account onto which the amount was added.</param>
/// <param name="NewIndex">The index the amount was assigned.</param>
/// <param name="EncryptedAmount">The encrypted amount that was added.</param>
public record NewEncryptedAmount(
    string AccountAddress,
    ulong NewIndex,
    string EncryptedAmount) : TransactionResultEvent;

public record TransferMemo(
    string DecodedText,
    TextDecodeType DecodeType,
    string RawHex) : TransactionResultEvent;

public enum TextDecodeType
{
    Cbor,
    None
}

/// <summary>
/// A transfer with schedule was enqueued.
/// </summary>
/// <param name="FromAccountAddress">Sender account address.</param>
/// <param name="ToAccountAddress">Receiver account address.</param>
/// <param name="AmountsSchedule">The list of releases. Ordered by increasing timestamp.</param>
public record TransferredWithSchedule(
    string FromAccountAddress,
    string ToAccountAddress,
    [property:UsePaging]
    TimestampedAmount[] AmountsSchedule) : TransactionResultEvent;

public record TimestampedAmount(DateTimeOffset Timestamp, ulong Amount);

public record UpdateEnqueued(
    DateTime EffectiveTime,
    string UpdateType) : TransactionResultEvent; // TODO: How to represent the updates - probably another large union!
