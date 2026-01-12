# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia tudo (evita erro de path/case)
COPY . .

# (Opcional) mostra estrutura nos logs pra debug
# RUN ls -la && find . -maxdepth 3 -type f -name "*.csproj"

# Restaura e publica a API
RUN dotnet restore EstudoIA.Api/EstudoIA.Api.csproj
RUN dotnet publish EstudoIA.Api/EstudoIA.Api.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EstudoIA.Api.dll"]
