using BankCore.Accounts.Domain.Entities;
using BankCore.Accounts.Infrastructure.Consumers;
using BankCore.Accounts.Infrastructure.Persistence;
using BankCore.Contracts.Events;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankCore.Accounts.UnitTests.Consumers;

public class TransactionRegisteredConsumerTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;
    private ServiceProvider _provider = null!;
    private ITestHarness _harness = null!;
    private Guid _accountId;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<AccountsDbContext>(options => options.UseSqlite(_connection));
        services.AddMassTransitTestHarness(cfg => cfg.AddConsumer<TransactionRegisteredConsumer>());

        _provider = services.BuildServiceProvider(validateScopes: true);
        _harness = _provider.GetRequiredService<ITestHarness>();

        using (var scope = _provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
            await db.Database.EnsureCreatedAsync();

            var account = Account.Open("1001-0", "Ada Lovelace", 100m);
            _accountId = account.Id;
            db.Accounts.Add(account);
            await db.SaveChangesAsync();
        }

        await _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        await _provider.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Deposit_credits_balance_and_publishes_TransactionApproved()
    {
        var transactionId = Guid.NewGuid();

        await _harness.Bus.Publish(new TransactionRegistered(
            transactionId, "Deposit", _accountId, null, 50m, DateTimeOffset.UtcNow));

        (await _harness.Published.Any<TransactionApproved>(x => x.Context.Message.TransactionId == transactionId))
            .Should().BeTrue();

        var balance = await GetBalanceAsync();
        balance.Should().Be(150m);
    }

    [Fact]
    public async Task Duplicate_message_is_not_applied_twice()
    {
        var transactionId = Guid.NewGuid();
        var message = new TransactionRegistered(transactionId, "Deposit", _accountId, null, 50m, DateTimeOffset.UtcNow);

        await _harness.Bus.Publish(message);
        (await _harness.Published.Any<TransactionApproved>(x => x.Context.Message.TransactionId == transactionId))
            .Should().BeTrue();

        await _harness.Bus.Publish(message);
        await Task.Delay(200);

        var balance = await GetBalanceAsync();
        balance.Should().Be(150m, "the duplicate message must be skipped due to idempotency");
    }

    [Fact]
    public async Task Withdrawal_with_insufficient_funds_publishes_TransactionRejected_and_does_not_change_balance()
    {
        var transactionId = Guid.NewGuid();

        await _harness.Bus.Publish(new TransactionRegistered(
            transactionId, "Withdrawal", _accountId, null, 1000m, DateTimeOffset.UtcNow));

        (await _harness.Published.Any<TransactionRejected>(x => x.Context.Message.TransactionId == transactionId))
            .Should().BeTrue();

        var balance = await GetBalanceAsync();
        balance.Should().Be(100m);
    }

    private async Task<decimal> GetBalanceAsync()
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        var account = await db.Accounts.FirstAsync(a => a.Id == _accountId);
        return account.Balance;
    }
}
