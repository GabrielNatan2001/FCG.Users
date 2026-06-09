FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/FCG.Users.Domain/FCG.Users.Domain.csproj", "src/FCG.Users.Domain/"]
COPY ["src/FCG.Users.Application/FCG.Users.Application.csproj", "src/FCG.Users.Application/"]
COPY ["src/FCG.Users.Infrastructure/FCG.Users.Infrastructure.csproj", "src/FCG.Users.Infrastructure/"]
COPY ["src/FCG.Users.API/FCG.Users.API.csproj", "src/FCG.Users.API/"]

RUN dotnet restore "src/FCG.Users.API/FCG.Users.API.csproj"

COPY src/ .
WORKDIR /src/FCG.Users.API
RUN dotnet publish "FCG.Users.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FCG.Users.API.dll"]
