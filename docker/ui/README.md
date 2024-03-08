# droneflightlogmvc

The Drone Flight Log is a personal UAV flight logging application implemented using .NET Core. It has the following components:

| Name | GitHub | Comments |
| --- | --- | --- |
| DroneFlightLogDb | [DroneFlightLogDb](https://github.com/davewalker5/DroneFlightLogDb) | Business logic, entities and database implementation |
| DroneFlightLog.Api | [DroneFlightLog.Api](https://github.com/davewalker5/DroneFlightLog.Api) | REST API for reading and writing to the database |
| DroneFlightLog.Mvc | [DroneFlightLog.Mvc](https://github.com/davewalker5/DroneFlightLog.Mvc) | ASP.NET MVC UI for accessing the database via the service |

The application provides facilities for recording and querying the following data:

* Operator details
* Drones, models and manufacturers
* Flights, flight locations and flight properties

The droneflightlogmvc image contains a build of the web-based MVC user interface.

## Getting Started

### Prerequisities

In order to run this image you'll need docker installed.

* [Windows](https://docs.docker.com/windows/started)
* [OS X](https://docs.docker.com/mac/started/)
* [Linux](https://docs.docker.com/linux/started/)

### Usage

#### Service Container Parameters

An instance of the dronefilghtlogapisqlite image must be started first in order for the UI to work. The recommended parameters are:

| Parameter | Value | Purpose |
| --- | --- | --- |
| -d | - | Run as a background  process
| -v | /local:/var/opt/droneflightlog.api-1.0.0.3 | Mount the host folder containing the SQLite database |
| --name | droneflightlogservice | Name the service so the UI can find it |
| --rm | - | Remove the container automatically when it stops |

The "/local" path given to the -v argument is described, below, and should be replaced with a value appropriate for the host running the container. 

The "--name" parameter is mandatory as the service URL is held in the application settings for the UI image and is expected to be:

http://droneflightlogservice:80

#### UI Container Parameters

The following "docker run" parameters are recommended when running the droneflightlogmvc image:

| Parameter | Value | Purpose |
| --- | --- | --- |
| -d | - | Run as a background  process
| -p | 5001:80 | Expose the container's port 80 as port 5001 on the host |
| --link | droneflightlogservice | Link to the drone flight log service container |
| --rm | - | Remove the container automatically when it stops |

For example:

```shell
docker run -d -p 5001:80 --rm --link droneflightlogservice davewalker5/droneflightlogmvc:latest
```

The port number "5001" can be replaced with any available port on the host.

#### Volumes

The description of the container parameters, above, specifies that a folder containing the SQLite database file for the Drone Flight Log is mounted in the running container, using the "-v" parameter.

That folder should contain a SQLite database that has been created using the instructions in the [Drone Flight Log wiki](https://github.com/davewalker5/DroneFlightLogDb/wiki).

Specifically, the following should be done:

- [Create the SQLite database](https://github.com/davewalker5/DroneFlightLogDb/wiki/Using-a-SQLite-Database)
- [Add a user to the database](https://github.com/davewalker5/DroneFlightLogDb/wiki/REST-API)

The folder containing the "droneflightlog.db" file can then be passed to the "docker run" command using the "-v" parameter.

#### Running the Image

To run the images for the service and UI, enter the following commands, substituting "/local" for the host folder containing the SQLite database, as described:

```shell
docker run -d -v  /local:/var/opt/droneflightlog.api-1.0.0.3/ --name droneflightlogservice --rm  davewalker5/droneflightlogapisqlite:latest
docker run -d -p 5001:80 --rm --link droneflightlogservice davewalker5/droneflightlogmvc:latest
```

Once the container is running, browse to the following URL on the host:

http://localhost:5001

You should see the login page for the UI.

## Built With

The droneflightlogmvc image was been built with the following:

| Aspect | Version |
| --- | --- |
| .NET Core CLI | 3.1.101 |
| Target Runtime | linux-x64 |
| Docker Desktop | 19.03.5, build 633a0ea |

## Find Us

* [DroneFlightLogDb on GitHub](https://github.com/davewalker5/DroneFlightLogDb)
* [DroneFlightLog.Api on GitHub](https://github.com/davewalker5/DroneFlightLog.Api)
* [DroneFlightLog.Mvc on GitHub](https://github.com/davewalker5/DroneFlightLog.Mvc)

## Versioning

For the versions available, see the [tags on this repository](https://github.com/davewalker5/DroneFlightLog.Mvc/tags).

## Authors

* **Dave Walker** - *Initial work* - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

See also the list of [contributors](https://github.com/davewalker5/DroneFlightLog.Mvc/contributors) who 
participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/davewalker5/DroneFlightLog.Mvc/blob/master/LICENSE) file for details.
