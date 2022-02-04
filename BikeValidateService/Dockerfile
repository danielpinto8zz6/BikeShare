FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BikeUnlockerService/BikeUnlockerService.csproj", "BikeUnlockerService/"]
RUN dotnet restore "BikeUnlockerService/BikeUnlockerService.csproj"
COPY . .
WORKDIR "/src/BikeUnlockerService"
RUN dotnet build "BikeUnlockerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BikeUnlockerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BikeUnlockerService.dll"]
