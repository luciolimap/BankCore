using BankCore.Transactions.Domain.Entities;
using BankCore.Transactions.Domain.Exceptions;
using FluentAssertions;

namespace BankCore.Transactions.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void Deposit_creates_pending_transaction()
    {
        var transaction = Transaction.Deposit(Guid.NewGuid(), 100m);

        transaction.Status.Should().Be(TransactionStatus.Pending);
        transaction.Type.Should().Be(TransactionType.Deposit);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Deposit_rejects_zero_or_negative_amount(decimal amount)
    {
        var act = () => Transaction.Deposit(Guid.NewGuid(), amount);

        act.Should().Throw<InvalidAmountException>();
    }

    [Fact]
    public void Transfer_rejects_same_source_and_destination()
    {
        var accountId = Guid.NewGuid();

        var act = () => Transaction.Transfer(accountId, accountId, 50m);

        act.Should().Throw<InvalidTransferException>();
    }

    [Fact]
    public void Transfer_rejects_empty_destination()
    {
        var act = () => Transaction.Transfer(Guid.NewGuid(), Guid.Empty, 50m);

        act.Should().Throw<InvalidTransferException>();
    }

    [Fact]
    public void Approve_moves_pending_transaction_to_approved()
    {
        var transaction = Transaction.Deposit(Guid.NewGuid(), 100m);

        transaction.Approve();

        transaction.Status.Should().Be(TransactionStatus.Approved);
        transaction.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_moves_pending_transaction_to_rejected_with_reason()
    {
        var transaction = Transaction.Withdrawal(Guid.NewGuid(), 100m);

        transaction.Reject("Insufficient funds");

        transaction.Status.Should().Be(TransactionStatus.Rejected);
        transaction.RejectionReason.Should().Be("Insufficient funds");
    }

    [Fact]
    public void Approve_from_a_non_pending_status_throws()
    {
        var transaction = Transaction.Deposit(Guid.NewGuid(), 100m);
        transaction.Reject("some reason");

        var act = transaction.Approve;

        act.Should().Throw<InvalidTransactionStateTransitionException>();
    }

    [Fact]
    public void Reject_from_a_non_pending_status_throws()
    {
        var transaction = Transaction.Deposit(Guid.NewGuid(), 100m);
        transaction.Approve();

        var act = () => transaction.Reject("too late");

        act.Should().Throw<InvalidTransactionStateTransitionException>();
    }
}
