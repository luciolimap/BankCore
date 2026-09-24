using BankCore.Transactions.Application.Abstractions;
using BankCore.Transactions.Infrastructure.Consumers;
using BankCore.Transactions.Infrastructure.Messaging;
using BankCore.Transactions.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankCore.Transactions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseProvider = configuration["Database:Provider"] ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("Default")
            ?? "Data Source=bankcore-transactions.db";

        services.AddDbContext<TransactionsDbContext>(options =>
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

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork>(sp => (IUnitOfWork)sp.GetRequiredService<ITransactionRepository>());
        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        var transport = configuration["Messaging:Transport"] ?? "InMemory";

        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<TransactionApprovedConsumer>();
            bus.AddConsumer<TransactionRejectedConsumer>();

            if (transport == "RabbitMq")
            {
                bus.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["Messaging:RabbitMq:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["Messaging:RabbitMq:Username"] ?? "guest");
                        h.Password(configuration["Messaging:RabbitMq:Password"] ?? "guest");
                    });

                    cfg.ReceiveEndpoint("transactions-status-updates", e =>
                    {
                        e.ConfigureConsumer<TransactionApprovedConsumer>(context);
                        e.ConfigureConsumer<TransactionRejectedConsumer>(context);
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
