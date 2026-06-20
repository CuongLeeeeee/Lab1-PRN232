# PRN232.StuPortal.API/Dockerfile

# -- Stage 1: Build ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + project files tru?c d? cache layer
COPY ["PRN232.StuPortal.API/PRN232.StuPortal.API.csproj",              "PRN232.StuPortal.API/"]
COPY ["PRN232.StuPortal.Services/PRN232.StuPortal.Services.csproj",    "PRN232.StuPortal.Services/"]
COPY ["PRN232.StuPortal.Repositories/PRN232.StuPortal.Repositories.csproj", "PRN232.StuPortal.Repositories/"]

RUN dotnet restore "PRN232.StuPortal.API/PRN232.StuPortal.API.csproj"

# Copy toàn b? source
COPY . .

WORKDIR "/src/PRN232.StuPortal.API"
RUN dotnet build "PRN232.StuPortal.API.csproj" -c Release -o /app/build

# -- Stage 2: Publish ----------------------------------------------------------
FROM build AS publish
RUN dotnet publish "PRN232.StuPortal.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -- Stage 3: Runtime ----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.StuPortal.API.dll"]