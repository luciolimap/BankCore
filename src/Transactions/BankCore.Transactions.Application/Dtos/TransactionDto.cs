namespace BankCore.Transactions.Application.Dtos;

public record TransactionDto(
    Guid Id,
    string Type,
    Guid SourceAccountId,
    Guid? DestinationAccountId,
    decimal Amount,
    string Status,
    string? RejectionReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);

public record DepositRequest(Guid AccountId, decimal Amount);
public record WithdrawalRequest(Guid AccountId, decimal Amount);
public record TransferRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Amount);
