FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ONLY the API project file
COPY ["LearnConnect.API/LearnConnect.API.csproj", "LearnConnect.API/"]

# Restore ONLY the API project
RUN dotnet restore "LearnConnect.API/LearnConnect.API.csproj"

# Copy all files (we will ignore tests during build)
COPY . .

# Build and publish ONLY the API
WORKDIR "/src/LearnConnect.API"
RUN dotnet publish "LearnConnect.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set Render default port setting
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LearnConnect.API.dll"]
