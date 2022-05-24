#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["database-service/database-service.csproj", "database-service/"]
RUN dotnet restore "database-service/database-service.csproj"
COPY . .
WORKDIR "/src/database-service"
RUN dotnet build "database-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "database-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "database-service.dll"]