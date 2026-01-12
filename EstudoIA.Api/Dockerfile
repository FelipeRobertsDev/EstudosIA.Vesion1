# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solution e os projetos
COPY *.sln .
COPY EstudoIA.Api/EstudoIA.Api.csproj EstudoIA.Api/
COPY EstudoIA.Vision1.Application/EstudoIA.Vision1.Application.csproj EstudoIA.Vision1.Application/
COPY EstudosIA.Vision1.ApplicationCommon/EstudosIA.Vision1.ApplicationCommon.csproj EstudosIA.Vision1.ApplicationCommon/

# Restaura dependências
RUN dotnet restore

# Copia o restante do código
COPY . .

# Publica a API
RUN dotnet publish EstudoIA.Api/EstudoIA.Api.csproj -c Release -o /app/publish

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Porta usada pelo Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copia build final
COPY --from=build /app/publish .

# Start da API
ENTRYPOINT ["dotnet", "EstudoIA.Api.dll"]
