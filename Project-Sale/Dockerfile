# שלב 1: בניית האפליקציה (Build Stage)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["server/server.csproj", "server/"]
RUN dotnet restore "server/server.csproj"

COPY . .
WORKDIR /src/server
RUN dotnet publish -c Release -o /app

# שלב 2: הרצת האפליקציה (Runtime Stage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "server.dll"]