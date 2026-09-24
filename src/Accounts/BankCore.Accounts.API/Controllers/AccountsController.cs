using BankCore.Accounts.Application.Dtos;
using BankCore.Accounts.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.Accounts.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController(AccountService accountService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> OpenAccount([FromBody] OpenAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await accountService.OpenAccountAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken cancellationToken = default)
    {
        var accounts = await accountService.ListAsync(skip, take, cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var account = await accountService.GetByIdAsync(id, cancellationToken);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpGet("{id:guid}/balance")]
    [ProducesResponseType(typeof(AccountBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(Guid id, CancellationToken cancellationToken)
    {
        var balance = await accountService.GetBalanceAsync(id, cancellationToken);
        return balance is null ? NotFound() : Ok(balance);
    }
}
