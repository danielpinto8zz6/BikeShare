FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DummyDockService/DummyDockService.csproj", "DummyDockService/"]
RUN dotnet restore "DummyDockService/DummyDockService.csproj"
COPY . .
WORKDIR "/src/DummyDockService"
RUN dotnet build "DummyDockService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DummyDockService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DummyDockService.dll"]
