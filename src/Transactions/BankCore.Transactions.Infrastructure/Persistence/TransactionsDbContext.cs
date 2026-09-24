using BankCore.Transactions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Transactions.Infrastructure.Persistence;

public class TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionsDbContext).Assembly);
    }
}
