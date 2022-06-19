FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TravelEventService/TravelEventService.csproj", "TravelEventService/"]
RUN dotnet restore "TravelEventService/TravelEventService.csproj"
COPY . .
WORKDIR "/src/TravelEventService"
RUN dotnet build "TravelEventService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TravelEventService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelEventService.dll"]
