using BankCore.Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankCore.Accounts.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Number).HasMaxLength(20).IsRequired();
        builder.HasIndex(a => a.Number).IsUnique();

        builder.Property(a => a.HolderName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Balance).HasColumnType("decimal(18,2)");
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
    }
}
