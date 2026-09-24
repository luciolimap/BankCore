using BankCore.Accounts.Domain.Exceptions;

namespace BankCore.Accounts.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string HolderName { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Account() { }

    private Account(Guid id, string number, string holderName, decimal initialDeposit, DateTimeOffset createdAt)
    {
        Id = id;
        Number = number;
        HolderName = holderName;
        Balance = initialDeposit;
        Status = AccountStatus.Active;
        CreatedAt = createdAt;
    }

    public static Account Open(string number, string holderName, decimal initialDeposit)
    {
        if (initialDeposit < 0)
        {
            throw new InvalidAmountException(initialDeposit);
        }

        return new Account(Guid.NewGuid(), number, holderName, initialDeposit, DateTimeOffset.UtcNow);
    }

    public void Block()
    {
        Status = AccountStatus.Blocked;
    }

    public void Credit(decimal amount)
    {
        EnsureActive();
        EnsurePositiveAmount(amount);

        Balance += amount;
    }

    public void Debit(decimal amount)
    {
        EnsureActive();
        EnsurePositiveAmount(amount);

        if (amount > Balance)
        {
            throw new InsufficientFundsException(Id, Balance, amount);
        }

        Balance -= amount;
    }

    private void EnsureActive()
    {
        if (Status != AccountStatus.Active)
        {
            throw new AccountNotActiveException(Id, Status.ToString());
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
