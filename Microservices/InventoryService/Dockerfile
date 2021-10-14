#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Microservices/InventoryService/InventoryService.csproj", "Microservices/InventoryService/"]
COPY ["Shared/HttpEnricher/HttpEnricher.csproj", "Shared/HttpEnricher/"]
COPY ["Shared/Services.Shared/Services.Shared.csproj", "Shared/Services.Shared/"]
RUN dotnet restore "Microservices/InventoryService/InventoryService.csproj"
COPY . .
WORKDIR "/src/Microservices/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]