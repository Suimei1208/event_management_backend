#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["user-services/user-services.csproj", "user-services/"]
RUN dotnet restore "./user-services/user-services.csproj"
COPY . .
WORKDIR "/src/user-services"
RUN dotnet build "./user-services.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./user-services.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "user-services.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS hot-reload
WORKDIR /app
RUN dotnet tool install -g dotnet-watch
COPY . .
ENTRYPOINT ["dotnet", "watch", "run", "--no-restore", "--urls", "http://0.0.0.0:5000"]