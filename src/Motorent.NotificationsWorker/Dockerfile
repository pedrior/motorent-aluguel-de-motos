FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Motorent.NotificationsWorker/Motorent.NotificationsWorker.csproj", "src/Motorent.NotificationsWorker/"]
COPY ["src/Motorent.Contracts/Motorent.Contracts.csproj", "src/Motorent.Contracts/"]
RUN dotnet restore "src/Motorent.NotificationsWorker/Motorent.NotificationsWorker.csproj"
COPY . .
WORKDIR "/src/src/Motorent.NotificationsWorker"
RUN dotnet build "Motorent.NotificationsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build
 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Motorent.NotificationsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Motorent.NotificationsWorker.dll"]
