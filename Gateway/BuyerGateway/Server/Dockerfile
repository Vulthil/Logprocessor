#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Gateway/BuyerGateway/Server/BuyerGateway.Server.csproj", "Gateway/BuyerGateway/Server/"]
COPY ["Gateway/BuyerGateway/Client/BuyerGateway.Client.csproj", "Gateway/BuyerGateway/Client/"]
COPY ["Shared/HttpEnricher/HttpEnricher.csproj", "Shared/HttpEnricher/"]
COPY ["Shared/Services.Shared/Services.Shared.csproj", "Shared/Services.Shared/"]
RUN dotnet restore "Gateway/BuyerGateway/Server/BuyerGateway.Server.csproj"
COPY . .
WORKDIR "/src/Gateway/BuyerGateway/Server"
RUN dotnet build "BuyerGateway.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BuyerGateway.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuyerGateway.Server.dll"]
