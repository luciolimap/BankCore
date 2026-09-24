namespace BankCore.Accounts.Application.Dtos;

public record AccountDto(
    Guid Id,
    string Number,
    string HolderName,
    decimal Balance,
    string Status,
    DateTimeOffset CreatedAt);

public record OpenAccountRequest(string HolderName, decimal InitialDeposit);

public record AccountBalanceDto(Guid AccountId, decimal Balance, DateTimeOffset AsOf);
