# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["LogisticsSaaS.sln", "./"]
COPY ["LogisticsSaaS.Web/LogisticsSaaS.Web.csproj", "LogisticsSaaS.Web/"]
COPY ["LogisticsSaaS.Core/LogisticsSaaS.Core.csproj", "LogisticsSaaS.Core/"]
COPY ["LogisticsSaaS.Infrastructure/LogisticsSaaS.Infrastructure.csproj", "LogisticsSaaS.Infrastructure/"]

RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR "/src/LogisticsSaaS.Web"
RUN dotnet build "LogisticsSaaS.Web.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "LogisticsSaaS.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port (Render uses PORT env var, but ASP.NET defaults to 8080 in .NET 8)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LogisticsSaaS.Web.dll"]
