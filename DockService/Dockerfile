FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DockService/DockService.csproj", "DockService/"]
RUN dotnet restore "DockService/DockService.csproj"
COPY . .
WORKDIR "/src/DockService"
RUN dotnet build "DockService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockService.dll"]
