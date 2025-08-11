FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/rinhabackend.Application/*.csproj ./src/rinhabackend.Application/
COPY src/rinhabackend.Infrastructure/*.csproj ./src/rinhabackend.Infrastructure/
COPY src/rinhabackend.API/*.csproj ./src/rinhabackend.API/

COPY . .
RUN dotnet restore rinhabackend.sln
RUN dotnet publish src/rinhabackend.API/rinhabackend.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "rinhabackend.API.dll"]
