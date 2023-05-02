FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /var/waterfront/src
COPY . .
RUN dotnet build -c Release -o /var/waterfront/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /srv/waterfront
COPY --from=build /var/waterfront/out .
ENTRYPOINT ["dotnet", "Waterfront.Server.dll"]
