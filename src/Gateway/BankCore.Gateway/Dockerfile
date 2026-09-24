FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Gateway/BankCore.Gateway/BankCore.Gateway.csproj", "src/Gateway/BankCore.Gateway/"]
RUN dotnet restore "src/Gateway/BankCore.Gateway/BankCore.Gateway.csproj"

COPY src/Gateway/BankCore.Gateway/ src/Gateway/BankCore.Gateway/

WORKDIR /src/src/Gateway/BankCore.Gateway
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "BankCore.Gateway.dll"]
