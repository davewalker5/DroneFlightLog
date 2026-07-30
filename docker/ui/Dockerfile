FROM mcr.microsoft.com/dotnet/core/aspnet:latest
ARG VERSION
COPY droneflightlog.mvc-${VERSION}.0 /opt/droneflightlog.mvc
WORKDIR /opt/droneflightlog.mvc/bin
ENTRYPOINT [ "./DroneFlightLog.Mvc" ]
