FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Reward.Worker/Reward.Worker.csproj", "Reward.Worker/"]
RUN dotnet restore "./Reward.Worker/Reward.Worker.csproj"
COPY . .
WORKDIR "/src/Reward.Worker"
RUN dotnet build "./Reward.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reward.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use SDK image again to avoid missing framework issue
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reward.Worker.dll"]