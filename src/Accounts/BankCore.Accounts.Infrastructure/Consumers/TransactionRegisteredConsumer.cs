using BankCore.Accounts.Domain.Entities;
using BankCore.Accounts.Domain.Exceptions;
using BankCore.Accounts.Infrastructure.Persistence;
using BankCore.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankCore.Accounts.Infrastructure.Consumers;

public class TransactionRegisteredConsumer(AccountsDbContext db, ILogger<TransactionRegisteredConsumer> logger)
    : IConsumer<TransactionRegistered>
{
    public async Task Consume(ConsumeContext<TransactionRegistered> consumeContext)
    {
        var message = consumeContext.Message;
        var cancellationToken = consumeContext.CancellationToken;

        var alreadyProcessed = await db.ProcessedMessages
            .AnyAsync(p => p.TransactionId == message.TransactionId, cancellationToken);

        if (alreadyProcessed)
        {
            logger.LogInformation("Transaction {TransactionId} already processed, skipping.", message.TransactionId);
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await ApplyToAccountsAsync(message, cancellationToken);

            db.ProcessedMessages.Add(new ProcessedMessage(message.TransactionId, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await consumeContext.Publish(new TransactionApproved(message.TransactionId, DateTimeOffset.UtcNow), cancellationToken);
        }
        catch (DomainException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogWarning(ex, "Transaction {TransactionId} rejected: {Reason}", message.TransactionId, ex.Message);

            await consumeContext.Publish(new TransactionRejected(message.TransactionId, ex.Message, DateTimeOffset.UtcNow), cancellationToken);
        }
    }

    private async Task ApplyToAccountsAsync(TransactionRegistered message, CancellationToken cancellationToken)
    {
        switch (message.Type)
        {
            case "Deposit":
            {
                var account = await GetAccountAsync(message.SourceAccountId, cancellationToken);
                account.Credit(message.Amount);
                break;
            }
            case "Withdrawal":
            {
                var account = await GetAccountAsync(message.SourceAccountId, cancellationToken);
                account.Debit(message.Amount);
                break;
            }
            case "Transfer":
            {
                var source = await GetAccountAsync(message.SourceAccountId, cancellationToken);
                var destination = await GetAccountAsync(message.DestinationAccountId!.Value, cancellationToken);
                source.Debit(message.Amount);
                destination.Credit(message.Amount);
                break;
            }
            default:
                throw new InvalidTransactionTypeException(message.Type);
        }
    }

    private async Task<Account> GetAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
        return account ?? throw new AccountNotFoundException(accountId);
    }
}
