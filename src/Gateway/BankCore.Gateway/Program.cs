var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/", () => Results.Content(
    """
    <h1>BankCore Gateway</h1>
    <ul>
      <li><a href="/accounts/swagger">Accounts API (Swagger)</a></li>
      <li><a href="/transactions/swagger">Transactions API (Swagger)</a></li>
    </ul>
    """,
    "text/html"));

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "gateway" }));

app.MapReverseProxy();

app.Run();
