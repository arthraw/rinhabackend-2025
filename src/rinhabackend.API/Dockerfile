FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../../rinhabackend.sln ./
COPY ../../src ./src               

RUN dotnet restore rinhabackend.sln

RUN dotnet publish src/rinhabackend.API/rinhabackend.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "rinhabackend.API.dll"]
