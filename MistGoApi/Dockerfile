# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MistGoApi.csproj", "."]
RUN dotnet restore "./MistGoApi.csproj"
COPY . .
RUN dotnet build "MistGoApi.csproj" -c Release -o /app/build
RUN dotnet publish "MistGoApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MistGoApi.dll"]