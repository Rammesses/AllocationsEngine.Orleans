#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
EXPOSE 11111
EXPOSE 30000
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Allocations.Engine.Worker/Allocations.Engine.Worker.csproj", "Allocations.Engine.Worker/"]
COPY ["Allocations.Engine.Grains.Interfaces/Allocations.Engine.Grains.Interfaces.csproj", "Allocations.Engine.Grains.Interfaces/"]
COPY ["Allocations.Engine.Grains/Allocations.Engine.Grains.csproj", "Allocations.Engine.Grains/"]
RUN dotnet restore "Allocations.Engine.Worker/Allocations.Engine.Worker.csproj"
COPY . .
WORKDIR "/src/Allocations.Engine.Worker"
RUN dotnet build "Allocations.Engine.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Allocations.Engine.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Allocations.Engine.Worker.dll"]
