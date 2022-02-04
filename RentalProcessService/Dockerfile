FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RentalProcessorService/RentalProcessorService.csproj", "RentalProcessorService/"]
RUN dotnet restore "RentalProcessorService/RentalProcessorService.csproj"
COPY . .
WORKDIR "/src/RentalProcessorService"
RUN dotnet build "RentalProcessorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RentalProcessorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RentalProcessorService.dll"]
