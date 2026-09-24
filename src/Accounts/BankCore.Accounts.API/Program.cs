using BankCore.Accounts.Application.Services;
using BankCore.Accounts.Domain.Entities;
using BankCore.Accounts.Domain.Exceptions;
using BankCore.Accounts.Infrastructure;
using BankCore.Accounts.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AccountService>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BankCore Accounts API v1");
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        var (statusCode, title) = exception switch
        {
            InsufficientFundsException or AccountNotActiveException or InvalidAmountException => (StatusCodes.Status409Conflict, exception.Message),
            AccountNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title
        });
    });
});

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "accounts-api" }));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

    if (db.Database.IsSqlServer())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }

    if (!await db.Accounts.AnyAsync())
    {
        db.Accounts.AddRange(
            Account.Open("1001-0", "Ada Lovelace", 1500m),
            Account.Open("1002-0", "Alan Turing", 800m));
        await db.SaveChangesAsync();
    }
}

app.Run();

public partial class Program;
