# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore first (cache layer)
COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"

# Copy all source and publish
COPY . .
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080 
ENTRYPOINT ["dotnet", "backend.dll"]
