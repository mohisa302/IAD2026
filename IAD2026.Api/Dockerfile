FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj files first 
COPY TaskManagement.Api/IAD2026.Api.csproj IAD2026.Api/
COPY TaskManagement.Application/IAD2026.Application.csproj IAD2026.Application/
COPY TaskManagement.Domain/IAD2026.Domain.csproj IAD2026.Domain/
COPY TaskManagement.Infrastructure/IAD2026.Infrastructure.csproj IAD2026.Infrastructure/

# restore solution
RUN dotnet restore IAD2026.Api/IAD2026.Api.csproj

# copy everything
COPY . .

RUN dotnet publish IAD2026.Api/IAD2026.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IAD2026.Api.dll"]