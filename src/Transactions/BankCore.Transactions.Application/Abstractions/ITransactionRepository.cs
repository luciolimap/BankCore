using BankCore.Transactions.Domain.Entities;

namespace BankCore.Transactions.Application.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> ListByAccountAsync(Guid accountId, CancellationToken cancellationToken);
}
