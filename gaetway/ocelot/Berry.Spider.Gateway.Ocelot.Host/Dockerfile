FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["gaetway/ocelot/Berry.Spider.Gateway.Ocelot.Host/Berry.Spider.Gateway.Ocelot.Host.csproj", "gaetway/ocelot/Berry.Spider.Gateway.Ocelot.Host/"]
RUN dotnet restore "gaetway/ocelot/Berry.Spider.Gateway.Ocelot.Host/Berry.Spider.Gateway.Ocelot.Host.csproj"
COPY . .
WORKDIR "/src/gaetway/ocelot/Berry.Spider.Gateway.Ocelot.Host"
RUN dotnet build "Berry.Spider.Gateway.Ocelot.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Berry.Spider.Gateway.Ocelot.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Berry.Spider.Gateway.Ocelot.Host.dll"]
