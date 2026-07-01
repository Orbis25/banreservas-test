FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["src/InventorySystem.Inventory/InventorySystem.Inventory.API/InventorySystem.Inventory.API.csproj", "src/InventorySystem.Inventory/InventorySystem.Inventory.API/"]
RUN dotnet restore "src/InventorySystem.Inventory/InventorySystem.Inventory.API/InventorySystem.Inventory.API.csproj"
COPY . .
WORKDIR "/src/src/InventorySystem.Inventory/InventorySystem.Inventory.API"
RUN dotnet build "./InventorySystem.Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./InventorySystem.Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem.Inventory.API.dll"]
