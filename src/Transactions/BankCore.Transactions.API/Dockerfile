FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Transactions/BankCore.Transactions.API/BankCore.Transactions.API.csproj", "src/Transactions/BankCore.Transactions.API/"]
COPY ["src/Transactions/BankCore.Transactions.Application/BankCore.Transactions.Application.csproj", "src/Transactions/BankCore.Transactions.Application/"]
COPY ["src/Transactions/BankCore.Transactions.Domain/BankCore.Transactions.Domain.csproj", "src/Transactions/BankCore.Transactions.Domain/"]
COPY ["src/Transactions/BankCore.Transactions.Infrastructure/BankCore.Transactions.Infrastructure.csproj", "src/Transactions/BankCore.Transactions.Infrastructure/"]
COPY ["src/BuildingBlocks/BankCore.Contracts/BankCore.Contracts.csproj", "src/BuildingBlocks/BankCore.Contracts/"]
RUN dotnet restore "src/Transactions/BankCore.Transactions.API/BankCore.Transactions.API.csproj"

COPY src/Transactions/BankCore.Transactions.API/ src/Transactions/BankCore.Transactions.API/
COPY src/Transactions/BankCore.Transactions.Application/ src/Transactions/BankCore.Transactions.Application/
COPY src/Transactions/BankCore.Transactions.Domain/ src/Transactions/BankCore.Transactions.Domain/
COPY src/Transactions/BankCore.Transactions.Infrastructure/ src/Transactions/BankCore.Transactions.Infrastructure/
COPY src/BuildingBlocks/BankCore.Contracts/ src/BuildingBlocks/BankCore.Contracts/

WORKDIR /src/src/Transactions/BankCore.Transactions.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "BankCore.Transactions.API.dll"]
