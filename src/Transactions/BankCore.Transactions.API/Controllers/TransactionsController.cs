using BankCore.Transactions.Application.Dtos;
using BankCore.Transactions.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.Transactions.API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController(TransactionService transactionService) : ControllerBase
{
    [HttpPost("deposits")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request, CancellationToken cancellationToken)
    {
        var transaction = await transactionService.DepositAsync(request, cancellationToken);
        return AcceptedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    [HttpPost("withdrawals")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawalRequest request, CancellationToken cancellationToken)
    {
        var transaction = await transactionService.WithdrawAsync(request, cancellationToken);
        return AcceptedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    [HttpPost("transfers")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken cancellationToken)
    {
        var transaction = await transactionService.TransferAsync(request, cancellationToken);
        return AcceptedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await transactionService.GetByIdAsync(id, cancellationToken);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListByAccount([FromQuery] Guid accountId, CancellationToken cancellationToken)
    {
        var transactions = await transactionService.ListByAccountAsync(accountId, cancellationToken);
        return Ok(transactions);
    }
}
