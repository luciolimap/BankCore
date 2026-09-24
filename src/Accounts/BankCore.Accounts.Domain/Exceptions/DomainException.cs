namespace BankCore.Accounts.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class InsufficientFundsException(Guid accountId, decimal balance, decimal requested)
    : DomainException($"Account {accountId} has insufficient funds: balance {balance}, requested {requested}.");

public sealed class AccountNotActiveException(Guid accountId, string status)
    : DomainException($"Account {accountId} is not active (status: {status}).");

public sealed class InvalidAmountException(decimal amount)
    : DomainException($"Amount must be greater than zero. Received: {amount}.");

public sealed class AccountNotFoundException(Guid accountId)
    : DomainException($"Account {accountId} was not found.");

public sealed class InvalidTransactionTypeException(string type)
    : DomainException($"Transaction type '{type}' is not recognized.");
