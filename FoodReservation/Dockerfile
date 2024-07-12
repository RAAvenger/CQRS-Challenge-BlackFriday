#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["FoodReservation/FoodReservation.Infrastructure.csproj", "FoodReservation/"]
COPY ["FoodReservation.Application/FoodReservation.Application.csproj", "FoodReservation.Application/"]
COPY ["FoodReservation.Domain/FoodReservation.Domain.csproj", "FoodReservation.Domain/"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
RUN dotnet restore "./FoodReservation/FoodReservation.Infrastructure.csproj"
COPY ["FoodReservation/", "FoodReservation/"]
COPY ["FoodReservation.Application/", "FoodReservation.Application/"]
COPY ["FoodReservation.Domain/", "FoodReservation.Domain/"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
WORKDIR "/src/FoodReservation"
RUN dotnet build "./FoodReservation.Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./FoodReservation.Infrastructure.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodReservation.Infrastructure.dll"]