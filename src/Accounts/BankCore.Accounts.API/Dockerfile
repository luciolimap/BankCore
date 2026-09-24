FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Accounts/BankCore.Accounts.API/BankCore.Accounts.API.csproj", "src/Accounts/BankCore.Accounts.API/"]
COPY ["src/Accounts/BankCore.Accounts.Application/BankCore.Accounts.Application.csproj", "src/Accounts/BankCore.Accounts.Application/"]
COPY ["src/Accounts/BankCore.Accounts.Domain/BankCore.Accounts.Domain.csproj", "src/Accounts/BankCore.Accounts.Domain/"]
COPY ["src/Accounts/BankCore.Accounts.Infrastructure/BankCore.Accounts.Infrastructure.csproj", "src/Accounts/BankCore.Accounts.Infrastructure/"]
COPY ["src/BuildingBlocks/BankCore.Contracts/BankCore.Contracts.csproj", "src/BuildingBlocks/BankCore.Contracts/"]
RUN dotnet restore "src/Accounts/BankCore.Accounts.API/BankCore.Accounts.API.csproj"

COPY src/Accounts/BankCore.Accounts.API/ src/Accounts/BankCore.Accounts.API/
COPY src/Accounts/BankCore.Accounts.Application/ src/Accounts/BankCore.Accounts.Application/
COPY src/Accounts/BankCore.Accounts.Domain/ src/Accounts/BankCore.Accounts.Domain/
COPY src/Accounts/BankCore.Accounts.Infrastructure/ src/Accounts/BankCore.Accounts.Infrastructure/
COPY src/BuildingBlocks/BankCore.Contracts/ src/BuildingBlocks/BankCore.Contracts/

WORKDIR /src/src/Accounts/BankCore.Accounts.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "BankCore.Accounts.API.dll"]
