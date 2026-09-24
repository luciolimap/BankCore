namespace BankCore.Contracts.Events;

public record TransactionRejected(
    Guid TransactionId,
    string Reason,
    DateTimeOffset ProcessedAt);
