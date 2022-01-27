FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FeedbackService/FeedbackService.csproj", "FeedbackService/"]
RUN dotnet restore "FeedbackService/FeedbackService.csproj"
COPY . .
WORKDIR "/src/FeedbackService"
RUN dotnet build "FeedbackService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FeedbackService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FeedbackService.dll"]
