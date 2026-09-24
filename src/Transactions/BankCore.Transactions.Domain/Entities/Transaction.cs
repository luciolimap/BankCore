using BankCore.Transactions.Domain.Exceptions;

namespace BankCore.Transactions.Domain.Entities;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public TransactionType Type { get; private set; }
    public Guid SourceAccountId { get; private set; }
    public Guid? DestinationAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private Transaction() { }

    private Transaction(TransactionType type, Guid sourceAccountId, Guid? destinationAccountId, decimal amount)
    {
        Id = Guid.NewGuid();
        Type = type;
        SourceAccountId = sourceAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        Status = TransactionStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Transaction Deposit(Guid accountId, decimal amount)
    {
        EnsurePositiveAmount(amount);
        return new Transaction(TransactionType.Deposit, accountId, null, amount);
    }

    public static Transaction Withdrawal(Guid accountId, decimal amount)
    {
        EnsurePositiveAmount(amount);
        return new Transaction(TransactionType.Withdrawal, accountId, null, amount);
    }

    public static Transaction Transfer(Guid sourceAccountId, Guid destinationAccountId, decimal amount)
    {
        EnsurePositiveAmount(amount);

        if (destinationAccountId == Guid.Empty)
        {
            throw new InvalidTransferException("A transfer must have a destination account.");
        }

        if (sourceAccountId == destinationAccountId)
        {
            throw new InvalidTransferException("Source and destination accounts must be different.");
        }

        return new Transaction(TransactionType.Transfer, sourceAccountId, destinationAccountId, amount);
    }

    public void Approve()
    {
        EnsurePending("approve");
        Status = TransactionStatus.Approved;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Reject(string reason)
    {
        EnsurePending("reject");
        Status = TransactionStatus.Rejected;
        RejectionReason = reason;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    private void EnsurePending(string attemptedTransition)
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new InvalidTransactionStateTransitionException(Status.ToString(), attemptedTransition);
        }
    }

    private static void EnsurePositiveAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidAmountException(amount);
        }
    }
}
