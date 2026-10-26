

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Ticketing-web/Ticketing-web.csproj", "Ticketing-web/"]
RUN dotnet restore "Ticketing-web/Ticketing-web.csproj"

COPY . .
WORKDIR /src/Ticketing-web

RUN dotnet publish "Ticketing-web.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ticketing-web.dll"]

