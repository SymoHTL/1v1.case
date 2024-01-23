FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Aspire/Aspire.AppHost/Aspire.AppHost.csproj", "Aspire/Aspire.AppHost/"]
COPY ["Aspire/Aspire.ServiceDefaults/Aspire.ServiceDefaults.csproj", "Aspire/Aspire.ServiceDefaults/"]
COPY ["Aspire.Web/Aspire.Web.csproj", "Aspire.Web/"]
COPY ["Aspire/Aspire.ApiService/Aspire.ApiService.csproj", "Aspire/Aspire.ApiService/"]
RUN dotnet restore "Aspire/Aspire.AppHost/Aspire.AppHost.csproj"
COPY . .
WORKDIR "/src/Aspire/Aspire.AppHost"
RUN dotnet build "Aspire.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Aspire.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aspire.AppHost.dll"]
