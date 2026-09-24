using BankCore.Contracts.Events;
using BankCore.Transactions.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankCore.Transactions.Infrastructure.Consumers;

public class TransactionRejectedConsumer(TransactionsDbContext db, ILogger<TransactionRejectedConsumer> logger)
    : IConsumer<TransactionRejected>
{
    public async Task Consume(ConsumeContext<TransactionRejected> context)
    {
        var message = context.Message;

        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.Id == message.TransactionId, context.CancellationToken);

        if (transaction is null)
        {
            logger.LogWarning("Received rejection for unknown transaction {TransactionId}.", message.TransactionId);
            return;
        }

        if (transaction.Status != Domain.Entities.TransactionStatus.Pending)
        {
            return;
        }

        transaction.Reject(message.Reason);
        await db.SaveChangesAsync(context.CancellationToken);
    }
}
