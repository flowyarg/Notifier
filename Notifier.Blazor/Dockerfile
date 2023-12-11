#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifier.Blazor/Notifier.Blazor.csproj", "Notifier.Blazor/"]
COPY ["Notifier.Vk/Notifier.Vk.csproj", "Notifier.Vk/"]
COPY ["Notifier.Logic/Notifier.Logic.csproj", "Notifier.Logic/"]
COPY ["Notifier.DataAccess/Notifier.DataAccess.csproj", "Notifier.DataAccess/"]
COPY ["Notifier.Telegram/Notifier.Telegram.csproj", "Notifier.Telegram/"]
RUN dotnet restore "./Notifier.Blazor/./Notifier.Blazor.csproj"
COPY . .
WORKDIR "/src/Notifier.Blazor"
RUN dotnet build "./Notifier.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notifier.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifier.Blazor.dll"]