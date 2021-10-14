#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

RUN mkdir -p /usr/share/man/man1 /usr/share/man/man2
RUN apt-get update && \
	apt-get install -y --no-install-recommends \
	openjdk-11-jre
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs
RUN java --version

WORKDIR /src
COPY ["LogProcessor/LogProcessor/LogProcessor.csproj", "LogProcessor/LogProcessor/"]
COPY ["LogProcessor/UI/Server/UI.Server.csproj", "LogProcessor/UI/Server/"]
COPY ["LogProcessor/UI/Client/UI.Client.csproj", "LogProcessor/UI/Client/"]
COPY ["LogProcessor/SessionTypes.Grammar/SessionTypes.Grammar.csproj", "LogProcessor/SessionTypes.Grammar/"]
COPY ["LogProcessor/LogProcessor.Model/LogProcessor.Model.csproj", "LogProcessor/LogProcessor.Model/"]
COPY ["Shared/Services.Shared/Services.Shared.csproj", "Shared/Services.Shared/"]
COPY ["LogProcessor/Automata/Automata.csproj", "LogProcessor/Automata/"]

RUN dotnet restore "LogProcessor/UI/Server/UI.Server.csproj"
RUN dotnet restore "LogProcessor/LogProcessor/LogProcessor.csproj"
COPY . .
WORKDIR "/src/LogProcessor/UI/Server"
RUN dotnet build "UI.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UI.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UI.Server.dll"]
