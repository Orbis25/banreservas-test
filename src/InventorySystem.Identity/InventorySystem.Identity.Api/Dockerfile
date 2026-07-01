FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["src/InventorySystem.Identity/InventorySystem.Identity.Api/InventorySystem.Identity.Api.csproj", "src/InventorySystem.Identity/InventorySystem.Identity.Api/"]
RUN dotnet restore "src/InventorySystem.Identity/InventorySystem.Identity.Api/InventorySystem.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/InventorySystem.Identity/InventorySystem.Identity.Api"
RUN dotnet build "./InventorySystem.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./InventorySystem.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem.Identity.Api.dll"]
