using BankCore.Contracts.Events;
using BankCore.Transactions.Application.Abstractions;
using BankCore.Transactions.Application.Dtos;
using BankCore.Transactions.Domain.Entities;

namespace BankCore.Transactions.Application.Services;

public class TransactionService(
    ITransactionRepository repository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher)
{
    public Task<TransactionDto> DepositAsync(DepositRequest request, CancellationToken cancellationToken) =>
        RegisterAsync(Transaction.Deposit(request.AccountId, request.Amount), cancellationToken);

    public Task<TransactionDto> WithdrawAsync(WithdrawalRequest request, CancellationToken cancellationToken) =>
        RegisterAsync(Transaction.Withdrawal(request.AccountId, request.Amount), cancellationToken);

    public Task<TransactionDto> TransferAsync(TransferRequest request, CancellationToken cancellationToken) =>
        RegisterAsync(
            Transaction.Transfer(request.SourceAccountId, request.DestinationAccountId, request.Amount),
            cancellationToken);

    public async Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await repository.GetByIdAsync(id, cancellationToken);
        return transaction is null ? null : ToDto(transaction);
    }

    public async Task<IReadOnlyList<TransactionDto>> ListByAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var transactions = await repository.ListByAccountAsync(accountId, cancellationToken);
        return transactions.Select(ToDto).ToList();
    }

    private async Task<TransactionDto> RegisterAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await repository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await eventPublisher.PublishTransactionRegisteredAsync(
            new TransactionRegistered(
                transaction.Id,
                transaction.Type.ToString(),
                transaction.SourceAccountId,
                transaction.DestinationAccountId,
                transaction.Amount,
                transaction.CreatedAt),
            cancellationToken);

        return ToDto(transaction);
    }

    private static TransactionDto ToDto(Transaction transaction) => new(
        transaction.Id,
        transaction.Type.ToString(),
        transaction.SourceAccountId,
        transaction.DestinationAccountId,
        transaction.Amount,
        transaction.Status.ToString(),
        transaction.RejectionReason,
        transaction.CreatedAt,
        transaction.CompletedAt);
}
