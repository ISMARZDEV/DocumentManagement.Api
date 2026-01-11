# 1. Base (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY ["Bhd.WebApi/Bhd.WebApi.csproj", "Bhd.WebApi/"]
COPY ["Bhd.Application/Bhd.Application.csproj", "Bhd.Application/"]
COPY ["Bhd.Domain/Bhd.Domain.csproj", "Bhd.Domain/"]
COPY ["Bhd.Infrastructure/Bhd.Infrastructure.csproj", "Bhd.Infrastructure/"]
RUN dotnet restore "Bhd.WebApi/Bhd.WebApi.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src/Bhd.WebApi"
RUN dotnet build "Bhd.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publicar
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Bhd.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bhd.WebApi.dll"]