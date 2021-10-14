#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Microservices/BankService/BankService.csproj", "Microservices/BankService/"]
COPY ["Shared/HttpEnricher/HttpEnricher.csproj", "Shared/HttpEnricher/"]
COPY ["Shared/Services.Shared/Services.Shared.csproj", "Shared/Services.Shared/"]
RUN dotnet restore "Microservices/BankService/BankService.csproj"
COPY . .
WORKDIR "/src/Microservices/BankService"
RUN dotnet build "BankService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankService.dll"]