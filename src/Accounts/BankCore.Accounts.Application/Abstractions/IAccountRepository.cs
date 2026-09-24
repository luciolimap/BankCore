using BankCore.Accounts.Domain.Entities;

namespace BankCore.Accounts.Application.Abstractions;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> ListAsync(int skip, int take, CancellationToken cancellationToken);
}
