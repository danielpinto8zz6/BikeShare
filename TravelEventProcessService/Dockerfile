FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TravelEventProcessService/TravelEventProcessService.csproj", "TravelEventProcessService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "TravelEventProcessService/TravelEventProcessService.csproj"
COPY . .
WORKDIR "/src/TravelEventProcessService"
RUN dotnet build "TravelEventProcessService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TravelEventProcessService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelEventProcessService.dll"]
