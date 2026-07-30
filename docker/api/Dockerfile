FROM mcr.microsoft.com/dotnet/core/aspnet:latest
ARG VERSION
COPY droneflightlog.api-${VERSION}.0 /opt/droneflightlog.api
WORKDIR /opt/droneflightlog.api/bin
ENTRYPOINT [ "./DroneFlightLog.Api" ]
