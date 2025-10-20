FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["DesafioPagueVeloz.Application/DesafioPagueVeloz.Application.csproj", "DesafioPagueVeloz.Application/"]
COPY ["DesafioPagueVeloz.Domain/DesafioPagueVeloz.Domain.csproj", "DesafioPagueVeloz.Domain/"]
COPY ["DesafioPagueVeloz.Persistense/DesafioPagueVeloz.Persistense.csproj", "DesafioPagueVeloz.Persistense/"]

COPY ["DesafioPagueVeloz.API/DesafioPagueVeloz.API.csproj", "DesafioPagueVeloz.API/"]
RUN dotnet restore "DesafioPagueVeloz.API/DesafioPagueVeloz.API.csproj"
COPY . .
WORKDIR "/src/DesafioPagueVeloz.API"
RUN dotnet build "./DesafioPagueVeloz.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DesafioPagueVeloz.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DesafioPagueVeloz.API.dll"]
