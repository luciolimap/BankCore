using BankCore.Contracts.Events;

namespace BankCore.Transactions.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishTransactionRegisteredAsync(TransactionRegistered @event, CancellationToken cancellationToken);
}
