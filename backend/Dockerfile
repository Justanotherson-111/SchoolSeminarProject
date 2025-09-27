# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /backend
# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore
# Copy the rest and build
COPY . ./
RUN dotnet publish -c Release -o /out
# Runtime
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime
WORKDIR /backend
COPY --from=build /out ./
# Expose port
EXPOSE 5000
# Start app
ENTRYPOINT [ "dotnet","backend.dll" ]
