FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RentalService/RentalService.csproj", "RentalService/"]
RUN dotnet restore "RentalService/RentalService.csproj"
COPY . .
WORKDIR "/src/RentalService"
RUN dotnet build "RentalService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RentalService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RentalService.dll"]
