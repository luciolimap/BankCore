using BankCore.Transactions.Application.Abstractions;
using BankCore.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Transactions.Infrastructure.Persistence;

public class TransactionRepository(TransactionsDbContext context) : ITransactionRepository, IUnitOfWork
{
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await context.Transactions.AddAsync(transaction, cancellationToken);
    }

    public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Transactions.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken cancellationToken) =>
        await context.Transactions
            .Where(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);
}
