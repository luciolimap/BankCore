using BankCore.Accounts.Application.Abstractions;
using BankCore.Accounts.Application.Dtos;
using BankCore.Accounts.Domain.Entities;

namespace BankCore.Accounts.Application.Services;

public class AccountService(IAccountRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<AccountDto> OpenAccountAsync(OpenAccountRequest request, CancellationToken cancellationToken)
    {
        var number = GenerateAccountNumber();
        var account = Account.Open(number, request.HolderName, request.InitialDeposit);

        await repository.AddAsync(account, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(account);
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(id, cancellationToken);
        return account is null ? null : ToDto(account);
    }

    public async Task<AccountBalanceDto?> GetBalanceAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(id, cancellationToken);
        return account is null
            ? null
            : new AccountBalanceDto(account.Id, account.Balance, DateTimeOffset.UtcNow);
    }

    public async Task<IReadOnlyList<AccountDto>> ListAsync(int skip, int take, CancellationToken cancellationToken)
    {
        var accounts = await repository.ListAsync(skip, take, cancellationToken);
        return accounts.Select(ToDto).ToList();
    }

    private static string GenerateAccountNumber() =>
        Random.Shared.Next(1000, 9999).ToString() + "-" + Random.Shared.Next(0, 9);

    private static AccountDto ToDto(Account account) => new(
        account.Id,
        account.Number,
        account.HolderName,
        account.Balance,
        account.Status.ToString(),
        account.CreatedAt);
}
