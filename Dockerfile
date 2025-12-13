# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar apenas arquivos de projeto necessários
COPY ["MoutsTI.API/MoutsTI.API.csproj", "MoutsTI.API/"]
COPY ["MoutsTI.Application/MoutsTI.Application.csproj", "MoutsTI.Application/"]
COPY ["MoutsTI.Domain/MoutsTI.Domain.csproj", "MoutsTI.Domain/"]
COPY ["MoutsTI.Dtos/MoutsTI.Dtos.csproj", "MoutsTI.Dtos/"]
COPY ["MoutsTI.Infra/MoutsTI.Infra.csproj", "MoutsTI.Infra/"]
COPY ["MoutsTI.Infra.Interfaces/MoutsTI.Infra.Interfaces.csproj", "MoutsTI.Infra.Interfaces/"]

# Restaurar dependências
RUN dotnet restore "MoutsTI.API/MoutsTI.API.csproj"

# Copiar código fonte (excluindo arquivos do .dockerignore)
COPY MoutsTI.API/ MoutsTI.API/
COPY MoutsTI.Application/ MoutsTI.Application/
COPY MoutsTI.Domain/ MoutsTI.Domain/
COPY MoutsTI.Dtos/ MoutsTI.Dtos/
COPY MoutsTI.Infra/ MoutsTI.Infra/
COPY MoutsTI.Infra.Interfaces/ MoutsTI.Infra.Interfaces/

# Build
WORKDIR "/src/MoutsTI.API"
RUN dotnet build "MoutsTI.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "MoutsTI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Criar usuário não-root para segurança adicional
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

# Healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MoutsTI.API.dll"]