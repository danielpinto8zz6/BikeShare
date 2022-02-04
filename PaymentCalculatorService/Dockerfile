FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PaymentCalculatorService/PaymentCalculatorService.csproj", "PaymentCalculatorService/"]
RUN dotnet restore "PaymentCalculatorService/PaymentCalculatorService.csproj"
COPY . .
WORKDIR "/src/PaymentCalculatorService"
RUN dotnet build "PaymentCalculatorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentCalculatorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentCalculatorService.dll"]
