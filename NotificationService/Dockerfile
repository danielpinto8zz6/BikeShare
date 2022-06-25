FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NotificationService/NotificationService.csproj", "NotificationService/"]
RUN dotnet restore "NotificationService/NotificationService.csproj"
COPY . .
WORKDIR "/src/NotificationService"
RUN dotnet build "NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["NotificationService/bikeshare-6b7f0-firebase-adminsdk-7ofm0-fd08b8cbba.json", ""]
ENV GOOGLE_APPLICATION_CREDENTIALS=/app/bikeshare-6b7f0-firebase-adminsdk-7ofm0-fd08b8cbba.json                                                                                               
ENTRYPOINT ["dotnet", "NotificationService.dll"]
