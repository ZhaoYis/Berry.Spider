FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Berry.Spider.TouTiao.Sender/Berry.Spider.TouTiao.Sender.csproj", "Berry.Spider.TouTiao.Sender/"]
RUN dotnet restore "src/Berry.Spider.TouTiao.Sender/Berry.Spider.TouTiao.Sender.csproj"
COPY . .
WORKDIR "/src/Berry.Spider.TouTiao.Sender"
RUN dotnet build "Berry.Spider.TouTiao.Sender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Berry.Spider.TouTiao.Sender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Berry.Spider.TouTiao.Sender.dll"]
