namespace BankCore.Contracts.Events;

public record TransactionRegistered(
    Guid TransactionId,
    string Type,
    Guid SourceAccountId,
    Guid? DestinationAccountId,
    decimal Amount,
    DateTimeOffset OccurredAt);
