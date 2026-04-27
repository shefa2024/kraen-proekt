FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["LearnConnect.API/LearnConnect.API.csproj", "LearnConnect.API/"]
COPY ["LearnConnect.Tests/LearnConnect.Tests.csproj", "LearnConnect.Tests/"]
COPY ["LearnConnect.sln", "./"]

# Restore packages
RUN dotnet restore "LearnConnect.sln"

# Copy the rest of the files
COPY . .

# Build and publish the API
WORKDIR "/src/LearnConnect.API"
RUN dotnet publish "LearnConnect.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set Render default port setting (Render sets PORT env variable, we use it)
# In .NET 8, we can bind to all interfaces on port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LearnConnect.API.dll"]
