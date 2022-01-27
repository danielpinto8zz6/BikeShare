FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PaymentValidatorService/PaymentValidatorService.csproj", "PaymentValidatorService/"]
RUN dotnet restore "PaymentValidatorService/PaymentValidatorService.csproj"
COPY . .
WORKDIR "/src/PaymentValidatorService"
RUN dotnet build "PaymentValidatorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentValidatorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentValidatorService.dll"]
