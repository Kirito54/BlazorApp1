# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY BlazorApp1/BlazorApp1/BlazorApp1.csproj BlazorApp1/BlazorApp1/
COPY BlazorApp1/BlazorApp1.Client/BlazorApp1.Client.csproj BlazorApp1/BlazorApp1.Client/
RUN dotnet restore BlazorApp1/BlazorApp1/BlazorApp1.csproj
COPY . .
WORKDIR /src/BlazorApp1/BlazorApp1
RUN dotnet publish BlazorApp1.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]
