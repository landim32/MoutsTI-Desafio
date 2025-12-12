# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["MoutsTI.API/MoutsTI.API.csproj", "MoutsTI.API/"]
COPY ["MoutsTI.Application/MoutsTI.Application.csproj", "MoutsTI.Application/"]
COPY ["MoutsTI.Domain/MoutsTI.Domain.csproj", "MoutsTI.Domain/"]
COPY ["MoutsTI.Dtos/MoutsTI.Dtos.csproj", "MoutsTI.Dtos/"]
COPY ["MoutsTI.Infra/MoutsTI.Infra.csproj", "MoutsTI.Infra/"]
COPY ["MoutsTI.Infra.Interfaces/MoutsTI.Infra.Interfaces.csproj", "MoutsTI.Infra.Interfaces/"]

RUN dotnet restore "MoutsTI.API/MoutsTI.API.csproj"

# Copiar todo o código fonte e build
COPY . .
WORKDIR "/src/MoutsTI.API"
RUN dotnet build "MoutsTI.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "MoutsTI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoutsTI.API.dll"]