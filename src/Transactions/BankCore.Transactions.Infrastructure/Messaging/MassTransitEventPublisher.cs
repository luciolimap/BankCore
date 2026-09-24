using BankCore.Contracts.Events;
using BankCore.Transactions.Application.Abstractions;
using MassTransit;

namespace BankCore.Transactions.Infrastructure.Messaging;

public class MassTransitEventPublisher(IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public Task PublishTransactionRegisteredAsync(TransactionRegistered @event, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(@event, cancellationToken);
}
