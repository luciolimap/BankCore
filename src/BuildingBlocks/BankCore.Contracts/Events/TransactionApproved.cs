namespace BankCore.Contracts.Events;

public record TransactionApproved(
    Guid TransactionId,
    DateTimeOffset ProcessedAt);
