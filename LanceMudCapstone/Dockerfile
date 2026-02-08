# ============================
# 1. Build Stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish the app
RUN dotnet publish -c Release -o /app/publish

# ============================
# 2. Runtime Stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port (Render defaults to 10000, but 8080 is fine)
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "LanceMudCapstone.dll"]
