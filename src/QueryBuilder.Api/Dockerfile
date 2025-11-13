# Usar imagem base do .NET 9.0 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["src/QueryBuilder.Api/QueryBuilder.Api.csproj", "QueryBuilder.Api/"]
COPY ["src/QueryBuilder.Domain/QueryBuilder.Domain.csproj", "QueryBuilder.Domain/"]
COPY ["src/QueryBuilder.Infra.Data/QueryBuilder.Infra.Data.csproj", "QueryBuilder.Infra.Data/"]
COPY ["src/QueryBuilder.Infra.CrossCutting/QueryBuilder.Infra.CrossCutting.csproj", "QueryBuilder.Infra.CrossCutting/"]
COPY ["src/QueryBuilder.Infra.CrossCutting.IoC/QueryBuilder.Infra.CrossCutting.IoC.csproj", "QueryBuilder.Infra.CrossCutting.IoC/"]
COPY ["src/QueryBuilder.Infra.Externals/QueryBuilder.Infra.Externals.csproj", "QueryBuilder.Infra.Externals/"]

RUN dotnet restore "QueryBuilder.Api/QueryBuilder.Api.csproj"

# Copiar todo o código fonte
COPY src/ .

# Build da aplicação
WORKDIR "/src/QueryBuilder.Api"
RUN dotnet build "QueryBuilder.Api.csproj" -c Release -o /app/build

# Publicar aplicação
FROM build AS publish
RUN dotnet publish "QueryBuilder.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueryBuilder.Api.dll"]
