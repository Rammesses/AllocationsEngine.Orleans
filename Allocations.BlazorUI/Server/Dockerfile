#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Allocations.BlazorUI/Server/Allocations.BlazorUI.Server.csproj", "Allocations.BlazorUI/Server/"]
COPY ["Allocations.Engine.Grains.Interfaces/Allocations.Engine.Grains.Interfaces.csproj", "Allocations.Engine.Grains.Interfaces/"]
COPY ["Allocations.BlazorUI/Client/Allocations.BlazorUI.Client.csproj", "Allocations.BlazorUI/Client/"]
COPY ["Allocations.BlazorUI/Shared/Allocations.BlazorUI.Shared.csproj", "Allocations.BlazorUI/Shared/"]
RUN dotnet restore "Allocations.BlazorUI/Server/Allocations.BlazorUI.Server.csproj"
COPY . .
WORKDIR "/src/Allocations.BlazorUI/Server"
RUN dotnet build "Allocations.BlazorUI.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Allocations.BlazorUI.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Allocations.BlazorUI.Server.dll"]