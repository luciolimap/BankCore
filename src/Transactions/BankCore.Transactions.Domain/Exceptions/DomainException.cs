namespace BankCore.Transactions.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class InvalidAmountException(decimal amount)
    : DomainException($"Amount must be greater than zero. Received: {amount}.");

public sealed class InvalidTransferException(string reason)
    : DomainException(reason);

public sealed class InvalidTransactionStateTransitionException(string currentStatus, string attemptedTransition)
    : DomainException($"Cannot {attemptedTransition} a transaction in status '{currentStatus}'.");
