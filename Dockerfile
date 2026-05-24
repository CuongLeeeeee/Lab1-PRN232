# ── Stage 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (layer cache for NuGet restore)
COPY StudentPortal.sln .
COPY StudentPortal.API/StudentPortal.API.csproj                   StudentPortal.API/
COPY StudentPortal.Services/StudentPortal.Services.csproj         StudentPortal.Services/
COPY StudentPortal.Repositories/StudentPortal.Repositories.csproj StudentPortal.Repositories/

# Restore NuGet packages
RUN dotnet restore StudentPortal.sln

# Copy all source code
COPY . .

# Publish the API project
RUN dotnet publish StudentPortal.API/StudentPortal.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose HTTP and HTTPS ports
EXPOSE 8080
EXPOSE 8081

# Set environment to Production by default (override in compose)
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "StudentPortal.API.dll"]
