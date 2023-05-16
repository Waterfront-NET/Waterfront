FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG SOURCEDIR
WORKDIR /srv/waterfront
COPY $SOURCEDIR .
ENTRYPOINT ["dotnet", "Waterfront.Server.dll"]
