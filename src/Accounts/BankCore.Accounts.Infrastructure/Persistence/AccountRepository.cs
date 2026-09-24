using BankCore.Accounts.Application.Abstractions;
using BankCore.Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Accounts.Infrastructure.Persistence;

public class AccountRepository(AccountsDbContext context) : IAccountRepository, IUnitOfWork
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken)
    {
        await context.Accounts.AddAsync(account, cancellationToken);
    }

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Account>> ListAsync(int skip, int take, CancellationToken cancellationToken) =>
        await context.Accounts
            .OrderBy(a => a.Number)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);
}
