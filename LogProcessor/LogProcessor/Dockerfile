#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

RUN mkdir -p /usr/share/man/man1 /usr/share/man/man2
RUN apt-get update && \
	apt-get install -y --no-install-recommends \
	openjdk-11-jre

RUN java --version

WORKDIR /src
COPY ["LogProcessor/LogProcessor/LogProcessor.csproj", "LogProcessor/LogProcessor/"]
COPY ["LogProcessor/LogProcessor.Model/LogProcessor.Model.csproj", "LogProcessor/LogProcessor.Model/"]
COPY ["LogProcessor/SessionTypes.Grammar/SessionTypes.Grammar.csproj", "LogProcessor/SessionTypes.Grammar/"]
COPY ["LogProcessor/Automata/Automata.csproj", "LogProcessor/Automata/"]
COPY ["Shared/Services.Shared/Services.Shared.csproj", "Shared/Services.Shared/"]
RUN dotnet restore "LogProcessor/LogProcessor/LogProcessor.csproj"
COPY . .
WORKDIR "/src/LogProcessor/LogProcessor"
RUN dotnet build "LogProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LogProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LogProcessor.dll"]