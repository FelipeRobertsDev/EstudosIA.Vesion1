FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copia tudo da solution
COPY . .

# restaura a solution inteira (nome EXATO da sua .sln)
RUN dotnet restore "EstudosIA.Vesion1.sln"

# build da solution inteira
RUN dotnet build "EstudosIA.Vesion1.sln" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# publica só a API
RUN dotnet publish "EstudoIA.Api/EstudoIA.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EstudoIA.Api.dll"]
