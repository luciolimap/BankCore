using BankCore.Accounts.Application.Abstractions;
using BankCore.Accounts.Infrastructure.Consumers;
using BankCore.Accounts.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankCore.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseProvider = configuration["Database:Provider"] ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("Default")
            ?? "Data Source=bankcore-accounts.db";

        services.AddDbContext<AccountsDbContext>(options =>
        {
            if (databaseProvider == "SqlServer")
            {
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork>(sp => (IUnitOfWork)sp.GetRequiredService<IAccountRepository>());

        var transport = configuration["Messaging:Transport"] ?? "InMemory";

        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<TransactionRegisteredConsumer>();

            if (transport == "RabbitMq")
            {
                bus.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["Messaging:RabbitMq:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["Messaging:RabbitMq:Username"] ?? "guest");
                        h.Password(configuration["Messaging:RabbitMq:Password"] ?? "guest");
                    });

                    cfg.ReceiveEndpoint("accounts-transaction-registered", e =>
                    {
                        e.ConfigureConsumer<TransactionRegisteredConsumer>(context);
                    });
                });
            }
            else
            {
                bus.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        return services;
    }
}
