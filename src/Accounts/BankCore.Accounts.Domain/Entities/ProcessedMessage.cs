namespace BankCore.Accounts.Domain.Entities;

public sealed class ProcessedMessage
{
    public Guid TransactionId { get; private set; }
    public DateTimeOffset ProcessedAt { get; private set; }

    private ProcessedMessage() { }

    public ProcessedMessage(Guid transactionId, DateTimeOffset processedAt)
    {
        TransactionId = transactionId;
        ProcessedAt = processedAt;
    }
}
