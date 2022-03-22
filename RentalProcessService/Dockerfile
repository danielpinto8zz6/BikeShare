FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RentalProcessService/RentalProcessService.csproj", "RentalProcessService/"]
RUN dotnet restore "RentalProcessService/RentalProcessService.csproj"
COPY . .
WORKDIR "/src/RentalProcessService"
RUN dotnet build "RentalProcessService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RentalProcessService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RentalProcessService.dll"]
