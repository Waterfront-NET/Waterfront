FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG CONFIGURATION
WORKDIR /srv/waterfront
COPY ./bin/$CONFIGURATION/net6.0 .
ENTRYPOINT ["dotnet", "Waterfront.Server.dll"]
