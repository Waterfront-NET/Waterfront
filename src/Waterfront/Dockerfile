FROM alpine:latest
WORKDIR /srv/waterfront
ARG SOURCEDIR
COPY $SOURCEDIR .
ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80
RUN apk add --no-cache libgcc libstdc++ icu-libs
ENTRYPOINT ["/srv/waterfront/Waterfront"]
