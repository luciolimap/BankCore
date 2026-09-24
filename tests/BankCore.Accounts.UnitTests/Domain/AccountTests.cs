using BankCore.Accounts.Domain.Entities;
using BankCore.Accounts.Domain.Exceptions;
using FluentAssertions;

namespace BankCore.Accounts.UnitTests.Domain;

public class AccountTests
{
    [Fact]
    public void Open_creates_active_account_with_initial_deposit_as_balance()
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        account.Balance.Should().Be(100m);
        account.Status.Should().Be(AccountStatus.Active);
    }

    [Fact]
    public void Credit_adds_to_balance()
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        account.Credit(50m);

        account.Balance.Should().Be(150m);
    }

    [Fact]
    public void Debit_subtracts_from_balance_when_funds_are_sufficient()
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        account.Debit(40m);

        account.Balance.Should().Be(60m);
    }

    [Fact]
    public void Debit_throws_InsufficientFundsException_when_amount_exceeds_balance()
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        var act = () => account.Debit(150m);

        act.Should().Throw<InsufficientFundsException>();
    }

    [Fact]
    public void Debit_does_not_change_balance_when_it_fails()
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        try { account.Debit(150m); } catch (InsufficientFundsException) { }

        account.Balance.Should().Be(100m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Debit_rejects_zero_or_negative_amounts(decimal amount)
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        var act = () => account.Debit(amount);

        act.Should().Throw<InvalidAmountException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Credit_rejects_zero_or_negative_amounts(decimal amount)
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);

        var act = () => account.Credit(amount);

        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Open_rejects_negative_initial_deposit()
    {
        var act = () => Account.Open("0001-1", "Ada Lovelace", -1m);

        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Operations_on_blocked_account_are_rejected()
    {
        var account = Account.Open("0001-1", "Ada Lovelace", 100m);
        account.Block();

        var debit = () => account.Debit(10m);
        var credit = () => account.Credit(10m);

        debit.Should().Throw<AccountNotActiveException>();
        credit.Should().Throw<AccountNotActiveException>();
    }
}
