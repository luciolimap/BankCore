using BankCore.Contracts.Events;
using BankCore.Transactions.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankCore.Transactions.Infrastructure.Consumers;

public class TransactionApprovedConsumer(TransactionsDbContext db, ILogger<TransactionApprovedConsumer> logger)
    : IConsumer<TransactionApproved>
{
    public async Task Consume(ConsumeContext<TransactionApproved> context)
    {
        var message = context.Message;

        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.Id == message.TransactionId, context.CancellationToken);

        if (transaction is null)
        {
            logger.LogWarning("Received approval for unknown transaction {TransactionId}.", message.TransactionId);
            return;
        }

        if (transaction.Status != Domain.Entities.TransactionStatus.Pending)
        {
            return;
        }

        transaction.Approve();
        await db.SaveChangesAsync(context.CancellationToken);
    }
}
